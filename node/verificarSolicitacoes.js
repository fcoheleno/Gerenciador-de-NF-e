const Firebird = require("node-firebird");

const options = {
    host: process.env.FIREBIRD_HOST || "localhost",
    port: Number(process.env.FIREBIRD_PORT || 3050),
    database: process.env.FIREBIRD_DATABASE,
    user: process.env.FIREBIRD_USER || "SYSDBA",
    password: process.env.FIREBIRD_PASSWORD
};

if (!options.database || !options.password) {
    console.error("Configure FIREBIRD_DATABASE e FIREBIRD_PASSWORD.");
    process.exitCode = 1;
} else {
    Firebird.attach(options, (err, db) => {
        if (err) {
            console.error("Falha na conexão:", err.message || err);
            process.exitCode = 1;
            return;
        }

        const sql = `
            SELECT
                TRIM(RF.RDB$FIELD_NAME) AS CAMPO,
                F.RDB$FIELD_TYPE AS TIPO,
                F.RDB$FIELD_LENGTH AS TAMANHO,
                RF.RDB$NULL_FLAG AS OBRIGATORIO
            FROM RDB$RELATION_FIELDS RF
            JOIN RDB$FIELDS F ON F.RDB$FIELD_NAME = RF.RDB$FIELD_SOURCE
            WHERE RF.RDB$RELATION_NAME = 'SOLICITACOES'
            ORDER BY RF.RDB$FIELD_POSITION
        `;

        db.query(sql, (queryErr, rows) => {
            if (queryErr) {
                console.error("Falha ao consultar os campos:", queryErr.message || queryErr);
                process.exitCode = 1;
            } else {
                console.log("Campos da tabela SOLICITACOES:");
                for (const row of rows) {
                    console.log(
                        `${row.CAMPO} | tipo=${row.TIPO} | tamanho=${row.TAMANHO} | obrigatório=${row.OBRIGATORIO === 1}`
                    );
                }
            }

            db.detach();
        });
    });
}
