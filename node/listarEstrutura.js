const Firebird = require("node-firebird");

const options = {
    host: process.env.FIREBIRD_HOST || "localhost",
    port: Number(process.env.FIREBIRD_PORT || 3050),
    database: process.env.FIREBIRD_DATABASE,
    user: process.env.FIREBIRD_USER || "SYSDBA",
    password: process.env.FIREBIRD_PASSWORD,
    lowercase_keys: false
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
            SELECT 'TABELA' AS TIPO, TRIM(RDB$RELATION_NAME) AS NOME
            FROM RDB$RELATIONS
            WHERE COALESCE(RDB$SYSTEM_FLAG, 0) = 0
              AND RDB$VIEW_BLR IS NULL

            UNION ALL

            SELECT 'PROCEDIMENTO' AS TIPO, TRIM(RDB$PROCEDURE_NAME) AS NOME
            FROM RDB$PROCEDURES
            WHERE COALESCE(RDB$SYSTEM_FLAG, 0) = 0

            ORDER BY 1, 2
        `;

        db.query(sql, (queryErr, rows) => {
            if (queryErr) {
                console.error("Falha ao listar a estrutura:", queryErr.message || queryErr);
                process.exitCode = 1;
            } else {
                console.log("Estrutura encontrada no banco:");
                for (const row of rows) {
                    console.log(`${row.TIPO}: ${row.NOME}`);
                }
            }

            db.detach();
        });
    });
}
