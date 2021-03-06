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
