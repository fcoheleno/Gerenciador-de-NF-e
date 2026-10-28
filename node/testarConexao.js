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
            console.error("Falha na conexão com Firebird.");
            console.error(err.message || err);
            process.exitCode = 1;
            return;
        }

        db.query("SELECT 1 AS TESTE FROM RDB$DATABASE", (queryErr, rows) => {
            if (queryErr) {
                console.error("Conexão aberta, mas a consulta de teste falhou.");
                console.error(queryErr.message || queryErr);
                process.exitCode = 1;
            } else {
                console.log("Conexão com Firebird funcionando!");
                console.log("Consulta de teste executada com sucesso.");
            }

            db.detach();
        });
    });
}
