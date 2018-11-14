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
-- Description:	Retorna tabelas de um schema.
-- =============================================
ALTER PROCEDURE [dbo].[PRC_SEL_TABELAS]
	@CATALOG NVARCHAR(100),
	@SCHEMA NVARCHAR(100)
AS
BEGIN
	DECLARE @CMD NVARCHAR(MAX)

	IF(@CATALOG IS NULL OR LEN(@CATALOG) = 0)
		SET @CMD = ''
	ELSE
		SET @CMD = ' USE ' + @CATALOG + ';'

	SET @CMD += '   SELECT TABLE_NAME
					FROM INFORMATION_SCHEMA.TABLES WITH(NOLOCK)
					WHERE UPPER(TABLE_SCHEMA) = UPPER(@SCHEMA)'

	EXEC SYS.SP_EXECUTESQL @CMD, N'@SCHEMA NVARCHAR(100)', @SCHEMA;
END
