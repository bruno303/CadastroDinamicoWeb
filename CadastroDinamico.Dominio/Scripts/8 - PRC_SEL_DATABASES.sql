USE [CAD_DINAMICO]
GO
/****** Object:  StoredProcedure [dbo].[PRC_SEL_TABELAS]    Script Date: 10/28/2018 6:29:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Bruno Oliveira
-- Create date: xx/xx/xxxx
-- Description:	Retorna databases da instância.
-- =============================================
CREATE PROCEDURE [dbo].[PRC_SEL_DATABASES]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [name]
	FROM master.sys.databases WITH(NOLOCK)
	WHERE database_id > 4 AND name <> 'CAD_DINAMICO';

	SET NOCOUNT OFF;
END
