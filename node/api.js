
const http = require("node:http");

const {
    consultarERegistrarTentativa,
    consultarStatusLimite,
    registrarBloqueioConsumo,
    consultarBloqueioConsumo
} = require("./src/rateLimiter");

const HOST = "127.0.0.1";
const PORT = Number(process.env.PORT || 3000);
const MAX_BODY = 16 * 1024;

function responder(res, status, dados) {
    res.writeHead(status, {
        "Content-Type": "application/json; charset=utf-8",
        "Cache-Control": "no-store"
    });

    res.end(JSON.stringify(dados));
}

function receberJson(req, res, callback) {
    let corpo = "";
    let excedeuLimite = false;

    req.on("data", (parte) => {
        if (excedeuLimite) return;

        corpo += parte.toString("utf8");

        if (Buffer.byteLength(corpo, "utf8") > MAX_BODY) {
            excedeuLimite = true;

            responder(res, 413, {
                erro: "Requisição muito grande."
            });

            req.destroy();
        }
    });

    req.on("end", () => {
        if (excedeuLimite) return;

        let dados;

        try {
            dados = JSON.parse(corpo);
        } catch {
            return responder(res, 400, {
                erro: "O corpo precisa ser um JSON válido."
            });
        }

        if (
            !dados ||
            typeof dados.cnpj !== "string" ||
            !/^\d{14}$/.test(dados.cnpj)
        ) {
            return responder(res, 400, {
                erro: "Informe um CNPJ com exatamente 14 números."
            });
        }

        callback(dados);
    });
}

async function consultarBloqueioOuResponder(cnpj, res) {
    const bloqueio = await consultarBloqueioConsumo(cnpj);

    if (!bloqueio.bloqueado) {
        return false;
    }

    responder(res, 200, {
        autorizada: false,
        bloqueioConsumo: true,
        total: 20,
        restantes: 0,
        segundosRestantes: bloqueio.segundosRestantes,
        tempoRestante: bloqueio.tempoRestante,
        liberacao: bloqueio.liberacao,
        mensagem:
            "CNPJ bloqueado por Consumo Indevido (cStat 656). " +
            "Tempo restante: " + bloqueio.tempoRestante + "."
    });

    return true;
}

const servidor = http.createServer((req, res) => {
    if (req.method !== "POST") {
        return responder(res, 404, {
            erro: "Endpoint não encontrado."
        });
    }

    if (req.url === "/api/consultas/status") {
        return receberJson(req, res, async (dados) => {
            try {
                if (
                    await consultarBloqueioOuResponder(
                        dados.cnpj,
                        res
                    )
                ) {
                    return;
                }

                const resultado =
                    await consultarStatusLimite(dados.cnpj);

                return responder(res, 200, resultado);
            } catch (erro) {
                console.error(
                    "Falha ao consultar status:",
                    erro.message
                );

                return responder(res, 503, {
                    autorizada: false,
                    erro: "Não foi possível verificar o limite."
                });
            }
        });
    }

    if (req.url === "/api/consultas/autorizar") {
        return receberJson(req, res, async (dados) => {
            try {
                if (
                    await consultarBloqueioOuResponder(
                        dados.cnpj,
                        res
                    )
                ) {
                    return;
                }

                const resultado =
                    await consultarERegistrarTentativa(dados.cnpj);

                return responder(res, 200, resultado);
            } catch (erro) {
                console.error(
                    "Falha no limitador:",
                    erro.message
                );

                return responder(res, 503, {
                    autorizada: false,
                    erro:
                        "Não foi possível verificar o limite. " +
                        "A consulta à SEFAZ não deve ser realizada."
                });
            }
        });
    }

    if (req.url === "/api/consultas/bloqueio-consumo") {
        return receberJson(req, res, async (dados) => {
            try {
                const resultado =
                    await registrarBloqueioConsumo(dados.cnpj);

                return responder(res, 200, resultado);
            } catch (erro) {
                console.error(
                    "Falha ao registrar bloqueio por cStat 656:",
                    erro.message
                );

                return responder(res, 503, {
                    registrado: false,
                    erro:
                        "Não foi possível registrar o bloqueio. " +
                        "Não realize novas consultas à SEFAZ."
                });
            }
        });
    }

    return responder(res, 404, {
        erro: "Endpoint não encontrado."
    });
});

servidor.listen(PORT, HOST, () => {
    console.log(
        `API do limitador disponível em http://${HOST}:${PORT}`
    );
});
