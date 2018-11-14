USE [CAD_DINAMICO]
GO
/****** Object:  StoredProcedure [dbo].[PRC_SEL_SCHEMAS]    Script Date: 10/28/2018 6:29:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Bruno Oliveira
-- Create date: xx/xx/xxxx
-- Description:	Retorna schemas de uma database.
-- =============================================
ALTER PROCEDURE [dbo].[PRC_SEL_SCHEMAS]
	@CATALOG NVARCHAR(100)
AS
BEGIN
	DECLARE @CMD NVARCHAR(MAX)

	IF(@CATALOG IS NULL OR LEN(@CATALOG) = 0)
		SET @CMD = ''
	ELSE
		SET @CMD = ' USE ' + @CATALOG + ';'

	SET @CMD += ' SELECT NAME FROM SYS.SCHEMAS WITH(NOLOCK) '

	EXEC SYS.SP_EXECUTESQL @CMD, N'@CATALOG NVARCHAR(100)', @CATALOG;
END
