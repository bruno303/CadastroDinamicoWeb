USE [CAD_DINAMICO];

CREATE TABLE dbo.LOG
(
	ID_LOG BIGINT PRIMARY KEY IDENTITY(1,1),
	ID_BANCO_DADOS INT,
    ID_ESQUEMA INT,
    ID_TABELA INT,
    USUARIO VARCHAR(500) NOT NULL,
    DATA_HORA DATETIME NOT NULL DEFAULT GETDATE(),
    METODO VARCHAR(200) NOT NULL,
    QUERY_EXECUTADA VARCHAR(MAX) NOT NULL,
    CONSTRAINT FK_LOG_BANCO_DADOS FOREIGN KEY (ID_BANCO_DADOS) REFERENCES BANCO_DADOS (ID_BANCO_DADOS),
    CONSTRAINT FK_LOG_ESQUEMA FOREIGN KEY (ID_ESQUEMA) REFERENCES ESQUEMA (ID_ESQUEMA),
    CONSTRAINT FK_LOG_TABLE FOREIGN KEY (ID_TABELA) REFERENCES TABELA (ID_TABELA)
);