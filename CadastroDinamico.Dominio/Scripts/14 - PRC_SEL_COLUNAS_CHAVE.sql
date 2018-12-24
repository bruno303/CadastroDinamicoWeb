USE [CAD_DINAMICO]
GO
/****** Object:  StoredProcedure [dbo].[PRC_SEL_CONFIGURACAO_TABELA]    Script Date: 10/28/2018 6:28:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Bruno Oliveira
-- Create date: 10/04/2018
-- Description:	Retorna colunas de chave estrangeira
-- =============================================
CREATE PROCEDURE [dbo].[PRC_SEL_COLUNAS_CHAVE]
	@I_ID_CONFIGURACAO_TABELA	INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CTC.COLUNA, CTC.COLUNA_EXIBICAO
	FROM CONFIGURACAO_TABELA_COLUNA CTC WITH(NOLOCK)
	INNER JOIN CONFIGURACAO_TABELA CT WITH(NOLOCK) ON CTC.ID_CONFIGURACAO_TABELA = CT.ID_CONFIGURACAO_TABELA
	INNER JOIN BANCO_DADOS BD WITH(NOLOCK) ON CT.ID_BANCO_DADOS = BD.ID_BANCO_DADOS
	INNER JOIN ESQUEMA ES WITH(NOLOCK) ON CT.ID_ESQUEMA = ES.ID_ESQUEMA
	INNER JOIN TABELA TB WITH(NOLOCK) ON CT.ID_TABELA = TB.ID_TABELA
	WHERE CTC.ID_CONFIGURACAO_TABELA = @I_ID_CONFIGURACAO_TABELA AND COALESCE(CTC.COLUNA_EXIBICAO, '') <> '';

	SET NOCOUNT OFF;
END
