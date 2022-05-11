CREATE TABLE Meter (
	FactoryNumber int PRIMARY KEY
	, LastCheck DateTime
	, NextCheck DateTime NOT NULL
	);

--one flat could have one meter
CREATE TABLE Flat (
	Street nvarchar(255) NOT NULL
	, Building int NOT NULL
	, FlatNumber int NOT NULL
	, MeterFactoryNumber int FOREIGN KEY REFERENCES Meter(FactoryNumber) NULL
	, PRIMARY KEY(Street, Building, FlatNumber)
	);

CREATE UNIQUE NONCLUSTERED INDEX idx_MeterFactoryNumber_notnull
ON Flat(MeterFactoryNumber)
WHERE MeterFactoryNumber IS NOT NULL;


--one meter could have many records
CREATE TABLE MeterRecords (
	MeterFactoryNumber int NOT NULL FOREIGN KEY REFERENCES Meter(FactoryNumber)
	, MeterValue int
	, CheckDate DateTime NOT NULL
	, PRIMARY KEY (MeterFactoryNumber, CheckDate)
	);

--AUDIT TABLES AND TRIGGERS
--audit table:
CREATE TABLE MeterReplacementHistory (
	Street nvarchar(255) NOT NULL
	, Building int NOT NULL
	, FlatNumber int NOT NULL
	, SetupDate DateTime NOT NULL
	, OldMeterValue int NULL
	, NewMeterFactoryNumber int FOREIGN KEY REFERENCES Meter(FactoryNumber) NULL
	, PRIMARY KEY (Street, Building, FlatNumber, SetupDate)
	, FOREIGN KEY (Street, Building, FlatNumber) REFERENCES Flat(Street, Building, FlatNumber)
	);
GO


--audit trigger:a
CREATE OR ALTER TRIGGER MeterReplacementTrigger 
ON Flat
AFTER INSERT, UPDATE
AS
BEGIN
	INSERT INTO MeterReplacementHistory (Street, Building, FlatNumber, SetupDate, OldMeterValue, NewMeterFactoryNumber)
	SELECT INSERTED.Street, INSERTED.Building, INSERTED.FlatNumber, GETDATE(), MeterRecords.MeterValue, INSERTED.MeterFactoryNumber
	FROM INSERTED 

	LEFT JOIN DELETED ON 
		INSERTED.Street = DELETED.Street 
		AND INSERTED.Building = DELETED.Building
		AND INSERTED.FlatNumber = DELETED.FlatNumber
	
	LEFT JOIN Meter ON DELETED.MeterFactoryNumber = Meter.FactoryNumber

	LEFT JOIN MeterRecords 
		ON Meter.FactoryNumber = MeterRecords.MeterFactoryNumber 
		AND Meter.LastCheck = MeterRecords.CheckDate

	WHERE INSERTED.MeterFactoryNumber != DELETED.MeterFactoryNumber 
		OR DELETED.MeterFactoryNumber IS NULL
END
GO

INSERT INTO Meter (FactoryNumber, NextCheck) VALUES (713357, '05-11-2020')
INSERT INTO Meter (FactoryNumber, NextCheck) VALUES (240760, '05-11-2020')
INSERT INTO Meter (FactoryNumber, NextCheck) VALUES (166832, '05-11-2020')

INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('soviet', 1, 1, NULL)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('soviet', 1, 2, NULL)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('soviet', 1, 3, NULL)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('soviet', 1, 4, NULL)

INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('soviet', 2, 1, NULL)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('soviet', 2, 2, 166832)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('soviet', 2, 3, NULL)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('soviet', 2, 4, NULL)

INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('karla-marksa', 1, 1, 713357)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('karla-marksa', 1, 2, NULL)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('karla-marksa', 1, 3, NULL)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('karla-marksa', 1, 4, NULL)

INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('karla-marksa', 2, 1, 240760)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('karla-marksa', 2, 2, NULL)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('karla-marksa', 2, 3, NULL)
INSERT INTO Flat (Street, Building, FlatNumber, MeterFactoryNumber) VALUES ('karla-marksa', 2, 4, NULL)