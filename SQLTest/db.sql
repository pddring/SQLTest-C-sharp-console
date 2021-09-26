CREATE TABLE People (
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	firstname varchar(255),
	lastname varchar(255)
);

CREATE TABLE Secrets (
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	PersonID INTEGER,
	secret varchar(255),
	FOREIGN KEY(PersonID) REFERENCES People(id)
);

INSERT INTO People (firstname, lastname) VALUES
('H', 'Potter'),
('H', 'Grainger'),
('R', 'Weasley');