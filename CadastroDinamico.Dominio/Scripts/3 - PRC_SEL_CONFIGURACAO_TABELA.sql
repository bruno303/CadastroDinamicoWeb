-- =============================================
-- Author:		Bruno Oliveira
-- Create date: 10/04/2018
-- Description:	Busca valores de configuração das tabelas
-- =============================================
CREATE PROCEDURE [dbo].[PRC_SEL_CONFIGURACAO_TABELA]
	@I_ID_CONFIGURACAO_TABELA	INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ID_CONFIGURACAO_TABELA, BANCO_DADOS, ESQUEMA, TABELA, JSON_COLUNAS_VISIVEIS, JSON_COLUNAS_CHAVE
	FROM CONFIGURACAO_TABELA WITH(NOLOCK)
	WHERE ID_CONFIGURACAO_TABELA = @I_ID_CONFIGURACAO_TABELA

	SET NOCOUNT OFF;
END
