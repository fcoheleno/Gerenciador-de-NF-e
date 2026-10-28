
const Firebird = require("node-firebird");

const LIMITE_TENTATIVAS = 20;
const JANELA_MINUTOS = 60;

function obterOpcoesFirebird() {
    const options = {
        host: process.env.FIREBIRD_HOST || "localhost",
        port: Number(process.env.FIREBIRD_PORT || 3050),
        database: process.env.FIREBIRD_DATABASE,
        user: process.env.FIREBIRD_USER || "SYSDBA",
        password: process.env.FIREBIRD_PASSWORD,
        lowercase_keys: false
    };

    if (!options.database || !options.password) {
        throw new Error(
            "Configure FIREBIRD_DATABASE e FIREBIRD_PASSWORD antes de iniciar o limitador."
        );
    }

    return options;
}

function validarCnpj(cnpj) {
    if (typeof cnpj !== "string" || !/^\d{14}$/.test(cnpj)) {
        throw new Error("O CNPJ deve conter exatamente 14 números.");
    }
}

function formatarTempo(segundos) {
    const total = Math.max(0, Math.ceil(segundos));
    const horas = Math.floor(total / 3600);
    const minutos = Math.floor((total % 3600) / 60);
    const segundosRestantes = total % 60;

    return [
        String(horas).padStart(2, "0"),
        String(minutos).padStart(2, "0"),
        String(segundosRestantes).padStart(2, "0")
    ].join(":");
}

function consultarERegistrarTentativa(cnpj) {
    validarCnpj(cnpj);

    return new Promise((resolve, reject) => {
        Firebird.attach(obterOpcoesFirebird(), (erroConexao, db) => {
            if (erroConexao) {
                return reject(new Error("Não foi possível conectar ao Firebird."));
            }

            db.transaction(Firebird.ISOLATION_READ_COMMITTED, (erroTransacao, transaction) => {
                if (erroTransacao) {
                    db.detach();
                    return reject(new Error("Não foi possível iniciar a transação."));
                }

                const finalizarComErro = (erro) => {
                    transaction.rollback(() => {
                        db.detach();
                        reject(erro);
                    });
                };

                // Esta atualização adquire a trava transacional compartilhada
                // por todas as chamadas do limitador.
                transaction.query(
                    "UPDATE SEFAZ_LIMITADOR_TRAVA SET VERSAO = VERSAO + 1 WHERE ID = 1",
                    [],
                    (erroTrava) => {
                        if (erroTrava) {
                            return finalizarComErro(
                                new Error("Não foi possível adquirir a trava do limitador.")
                            );
                        }

                        const sql = `
                            SELECT
                                COUNT(*) AS TOTAL,
                                MIN(DATA_HORA) AS MAIS_ANTIGA
                            FROM SEFAZ_LIMITADOR_TENTATIVAS
                            WHERE CNPJ = ?
                              AND DATA_HORA > DATEADD(-60 MINUTE TO CURRENT_TIMESTAMP)
                        `;

                        transaction.query(sql, [cnpj], (erroConsulta, linhas) => {
                            if (erroConsulta) {
                                return finalizarComErro(
                                    new Error("Falha ao consultar o histórico de tentativas.")
                                );
                            }

                            const total = Number(linhas[0].TOTAL || 0);

                            if (total >= LIMITE_TENTATIVAS) {
                                transaction.rollback((erroRollback) => {
                                    db.detach();

                                    if (erroRollback) {
                                        return reject(new Error("Falha ao encerrar a transação."));
                                    }

                                    const antiga = linhas[0].MAIS_ANTIGA;
                                    const liberacao = new Date(
                                        new Date(antiga).getTime() +
                                        JANELA_MINUTOS * 60 * 1000
                                    );

                                    const segundosRestantes = Math.max(
                                        0,
                                        Math.ceil((liberacao.getTime() - Date.now()) / 1000)
                                    );

                                    resolve({
                                        autorizada: false,
                                        total,
                                        segundosRestantes,
                                        tempoRestante: formatarTempo(segundosRestantes),
                                        liberacao: liberacao.toISOString(),
                                        mensagem: `Limite de ${LIMITE_TENTATIVAS} tentativas atingido. Tempo restante: ${formatarTempo(segundosRestantes)}.`
                                    });
                                });

                                return;
                            }

                            transaction.query(
                                `INSERT INTO SEFAZ_LIMITADOR_TENTATIVAS (CNPJ, DATA_HORA)
                                 VALUES (?, CURRENT_TIMESTAMP)`,
                                [cnpj],
                                (erroInsercao) => {
                                    if (erroInsercao) {
                                        return finalizarComErro(
                                            new Error("Não foi possível registrar a tentativa.")
                                        );
                                    }

                                    transaction.commit((erroCommit) => {
                                        db.detach();

                                        if (erroCommit) {
                                            return reject(new Error("Não foi possível confirmar a tentativa no banco."));
                                        }

                                        resolve({
                                            autorizada: true,
                                            total: total + 1,
                                            segundosRestantes: 0,
                                            tempoRestante: "00:00:00",
                                            liberacao: null,
                                            mensagem: "Tentativa autorizada pelo limitador."
                                        });
                                    });
                                }
                            );
                        });
                    }
                );
            });
        });
    });
}

function consultarStatusLimite(cnpj) {
    validarCnpj(cnpj);

    return new Promise((resolve, reject) => {
        Firebird.attach(obterOpcoesFirebird(), (erroConexao, db) => {
            if (erroConexao) {
                return reject(
                    new Error("Não foi possível conectar ao Firebird.")
                );
            }

            const encerrarBanco = () => {
                db.detach();
            };

            const sql = `
                SELECT
                    COUNT(*) AS TOTAL,
                    MIN(DATA_HORA) AS MAIS_ANTIGA
                FROM SEFAZ_LIMITADOR_TENTATIVAS
                WHERE CNPJ = ?
                  AND DATA_HORA > DATEADD(-60 MINUTE TO CURRENT_TIMESTAMP)
            `;

            db.query(sql, [cnpj], (erroConsulta, linhas) => {
                if (erroConsulta) {
                    encerrarBanco();
                    return reject(
                        new Error("Falha ao consultar o histórico de tentativas.")
                    );
                }

                const total = Number(linhas[0].TOTAL || 0);

                if (total < LIMITE_TENTATIVAS) {
                    encerrarBanco();

                    return resolve({
                        autorizada: true,
                        total,
                        restantes: LIMITE_TENTATIVAS - total,
                        segundosRestantes: 0,
                        tempoRestante: "00:00:00",
                        mensagem: "Cliente disponível para consulta."
                    });
                }

                const antiga = linhas[0].MAIS_ANTIGA;

                if (!antiga) {
                    encerrarBanco();
                    return reject(
                        new Error("Não foi possível calcular o tempo restante.")
                    );
                }

                const liberacao = new Date(
                    new Date(antiga).getTime() +
                    JANELA_MINUTOS * 60 * 1000
                );

                const segundosRestantes = Math.max(
                    0,
                    Math.ceil(
                        (liberacao.getTime() - Date.now()) / 1000
                    )
                );

                encerrarBanco();

                resolve({
                    autorizada: segundosRestantes === 0,
                    total,
                    restantes: 0,
                    segundosRestantes,
                    tempoRestante: formatarTempo(segundosRestantes),
                    liberacao: liberacao.toISOString(),
                    mensagem: segundosRestantes > 0
                        ? `Limite atingido. Tempo restante: ${formatarTempo(segundosRestantes)}.`
                        : "Cliente disponível para consulta."
                });
            });
        });
    });
}


function registrarBloqueioConsumo(cnpj) {
    validarCnpj(cnpj);

    return new Promise((resolve, reject) => {
        Firebird.attach(obterOpcoesFirebird(), (erroConexao, db) => {
            if (erroConexao) {
                return reject(new Error("Não foi possível conectar ao Firebird."));
            }

            const sql = `
                UPDATE OR INSERT INTO SEFAZ_BLOQUEIO_CONSUMO
                    (CNPJ, DATA_HORA_BLOQUEIO)
                VALUES (?, CURRENT_TIMESTAMP)
                MATCHING (CNPJ)
            `;

            db.query(sql, [cnpj], (erro) => {
                db.detach();

                if (erro) {
                    return reject(new Error("Não foi possível registrar o bloqueio por Consumo Indevido."));
                }

                resolve({
                    registrado: true,
                    mensagem: "Bloqueio por Consumo Indevido registrado por uma hora."
                });
            });
        });
    });
}

function consultarBloqueioConsumo(cnpj) {
    validarCnpj(cnpj);

    return new Promise((resolve, reject) => {
        Firebird.attach(obterOpcoesFirebird(), (erroConexao, db) => {
            if (erroConexao) {
                return reject(new Error("Não foi possível conectar ao Firebird."));
            }

            const sql = `
                SELECT DATA_HORA_BLOQUEIO
                FROM SEFAZ_BLOQUEIO_CONSUMO
                WHERE CNPJ = ?
            `;

            db.query(sql, [cnpj], (erro, linhas) => {
                db.detach();

                if (erro) {
                    return reject(new Error("Não foi possível consultar o bloqueio por Consumo Indevido."));
                }

                if (!linhas || linhas.length === 0) {
                    return resolve({
                        bloqueado: false,
                        segundosRestantes: 0,
                        tempoRestante: "00:00:00"
                    });
                }

                const inicio = new Date(linhas[0].DATA_HORA_BLOQUEIO);
                const liberacao = new Date(inicio.getTime() + 60 * 60 * 1000);
                const segundosRestantes = Math.max(
                    0,
                    Math.ceil((liberacao.getTime() - Date.now()) / 1000)
                );

                resolve({
                    bloqueado: segundosRestantes > 0,
                    segundosRestantes,
                    tempoRestante: formatarTempo(segundosRestantes),
                    liberacao: liberacao.toISOString()
                });
            });
        });
    });
}

module.exports = {
    consultarERegistrarTentativa,
    consultarStatusLimite,
    registrarBloqueioConsumo,
    consultarBloqueioConsumo
};