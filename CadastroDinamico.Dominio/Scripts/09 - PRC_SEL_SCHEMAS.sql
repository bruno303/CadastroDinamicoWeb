-- =============================================
-- Author:		Bruno Oliveira
-- Create date: xx/xx/xxxx
-- Description:	Retorna schemas de uma database.
-- =============================================
CREATE PROCEDURE [dbo].[PRC_SEL_SCHEMAS]
	@CATALOG NVARCHAR(100)
AS
BEGIN
	DECLARE @CMD NVARCHAR(MAX)

	IF(@CATALOG IS NULL OR LEN(@CATALOG) = 0)
		SET @CMD = '';
	ELSE
		SET @CMD = ' USE ' + @CATALOG + ';';

	SET @CMD += ' SELECT NAME FROM SYS.SCHEMAS WITH(NOLOCK) ';
	SET @CMD += ' WHERE NAME NOT IN (';
	SET @CMD += '     ''guest'', ';
	SET @CMD += '     ''INFORMATION_SCHEMA'', ';
	SET @CMD += '     ''sys'', ';
	SET @CMD += '     ''db_owner'', ';
	SET @CMD += '     ''db_accessadmin'', ';
	SET @CMD += '     ''db_securityadmin'', ';
	SET @CMD += '     ''db_ddladmin'', ';
	SET @CMD += '     ''db_backupoperator'', ';
	SET @CMD += '     ''db_datareader'', ';
	SET @CMD += '     ''db_datawriter'', ';
	SET @CMD += '     ''db_denydatareader'', ';
	SET @CMD += '     ''db_denydatawriter'' )';

	EXEC SYS.SP_EXECUTESQL @CMD, N'@CATALOG NVARCHAR(100)', @CATALOG;
END
