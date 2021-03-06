-- =============================================
-- Author:		Bruno Oliveira
-- Create date: xx/xx/xxxx
-- Description:	Retorna tabelas de um schema.
-- =============================================
CREATE PROCEDURE [dbo].[PRC_SEL_TABELAS]
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
