
const { consultarERegistrarTentativa } = require("./src/rateLimiter");

async function main() {
    // CNPJ fictício de 14 dígitos, usado somente para o teste.
    const cnpjTeste = "12345678000195";

    try {
        const resultado = await consultarERegistrarTentativa(cnpjTeste);
        console.log("Resultado do limitador:");
        console.log(JSON.stringify(resultado, null, 2));
    } catch (erro) {
        console.error("Erro no teste:", erro.message);
        process.exitCode = 1;
    }
}

main();