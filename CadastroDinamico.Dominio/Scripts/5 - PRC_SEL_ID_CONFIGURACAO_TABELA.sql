-- =============================================
-- Author:		Bruno Oliveira
-- Create date: 10/04/2018
-- Description:	Busca um registro de configuração das tabelas para determinada tabela
-- =============================================
CREATE PROCEDURE [dbo].[PRC_SEL_ID_CONFIGURACAO_TABELA]
	@I_BANCO_DADOS				NVARCHAR(120),
	@I_ESQUEMA					NVARCHAR(120),
	@I_TABELA					NVARCHAR(120)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ID_CONFIGURACAO_TABELA
	FROM CONFIGURACAO_TABELA WITH(NOLOCK)
	WHERE BANCO_DADOS = @I_BANCO_DADOS AND ESQUEMA = @I_ESQUEMA AND TABELA = @I_TABELA

	SET NOCOUNT OFF;
END
