﻿--@(#) script.ddl


CREATE TABLE lytis
(
	id int IDENTITY(1,1),
	name varchar (255) NOT NULL,
	PRIMARY KEY(id)
);
INSERT INTO lytis(id, name) VALUES(1, 'vyras');
INSERT INTO lytis(id, name) VALUES(2, 'moteris');
INSERT INTO lytis(id, name) VALUES(3, 'kita');

CREATE TABLE mokėjimostatusas
(
	id int IDENTITY(1,1),
	name varchar (255),
	PRIMARY KEY(id)
);


CREATE TABLE mokejimotipas
(
	id int IDENTITY(1,1),
	name varchar (255) NOT NULL,
	PRIMARY KEY(id)
);
INSERT INTO mokejimotipas(id, name) VALUES(1, 'kortelė+parduotuvės_kreditas');
INSERT INTO mokejimotipas(id, name) VALUES(2, 'parduotuvės_kreditas');
INSERT INTO mokejimotipas(id, name) VALUES(3, 'kortelė');
INSERT INTO mokejimotipas(id, name) VALUES(4, 'paypal');
INSERT INTO mokejimotipas(id, name) VALUES(5, 'paypal+parduotuvės_kreditas');

CREATE TABLE nuolaidosstatusas
(
	id int IDENTITY(1,1),
	name varchar (255) NOT NULL,
	PRIMARY KEY(id)
);
INSERT INTO nuolaidosstatusas(id, name) VALUES(1, 'suplanuotas');
INSERT INTO nuolaidosstatusas(id, name) VALUES(2, 'aktyvus');
INSERT INTO nuolaidosstatusas(id, name) VALUES(3, 'pasibaigęs');

CREATE TABLE nuolaidossukūrimobūdas
(
	id int IDENTITY(1,1),
	name varchar (255) NOT NULL,
	PRIMARY KEY(id)
);
INSERT INTO nuolaidossukūrimobūdas(id, name) VALUES(1, 'automatinis');
INSERT INTO nuolaidossukūrimobūdas(id, name) VALUES(2, 'rankinis');

CREATE TABLE sandelis
(
	id int IDENTITY(1,1),
	pavadinimas varchar (255) NOT NULL,
	vieta varchar (255),
	PRIMARY KEY(id)
);

CREATE TABLE vartotojas
(
	age int,
	id int IDENTITY(1,1),,
	vardas varchar (255),
	pavarde varchar (255),
	slaptazodis varchar (255),
	telefononumeris varchar (255),
	PRIMARY KEY(id)
);

CREATE TABLE siuntimostatusas
(
	id int IDENTITY(1,1),
	name varchar (255) NOT NULL,
	PRIMARY KEY(id)
);
INSERT INTO siuntimostatusas(id, name) VALUES(1, 'laukiama');
INSERT INTO siuntimostatusas(id, name) VALUES(2, 'išsiųsta');
INSERT INTO siuntimostatusas(id, name) VALUES(3, 'priduota');
INSERT INTO siuntimostatusas(id, name) VALUES(4, 'atšaukta');

CREATE TABLE administratorius
(
	id int IDENTITY(1,1),,
	arvadovas BIT NOT NULL,
	atlyginimas float,
	kortelesnumeris varchar (255),
	PRIMARY KEY(id),
	FOREIGN KEY(id) REFERENCES vartotojas (id)
);

CREATE TABLE kategorija
(
	id int IDENTITY(1,1),,
	pavadinimas varchar (255) NOT NULL,
	aprasas VARCHAR(MAX)  NOT NULL,
	fk_tevinekategorija int,
	PRIMARY KEY(id),
	CONSTRAINT Priklauso FOREIGN KEY(fk_tevinekategorija) REFERENCES kategorija (id)
);

CREATE TABLE nuolaidoskodas
(
	id int IDENTITY(1,1),
	sukurimodata date,
	veikimopradziosdata date NULL,
	panaudojimuskaičius int NULL,
	kodas varchar (255),
	verte float,
	aprasymas varchar (255) NULL,
	pavadinimas varchar (255) NULL,
	galiojimopabaigosdata date NULL,
	yraribotas BIT,
	statusas int,
	sukurimobudas int,
	PRIMARY KEY(id),
	FOREIGN KEY(statusas) REFERENCES nuolaidosstatusas (id),
	FOREIGN KEY(sukurimobudas) REFERENCES nuolaidossukūrimobūdas (id)
);

CREATE TABLE pardavejas
(
	id int IDENTITY(1,1),
	vieta varchar (255),
	PRIMARY KEY(id),
	FOREIGN KEY(id) REFERENCES vartotojas (id)
);

CREATE TABLE atlyginimas
(
	mokestis float ,
	bonusas float ,
	fk_administratorius int NOT NULL,
	fk_administratorius1 int NOT NULL,
	PRIMARY KEY(fk_administratorius, fk_administratorius1),
	CONSTRAINT Gauna FOREIGN KEY(fk_administratorius) REFERENCES administratorius (id),
	CONSTRAINT Moka FOREIGN KEY(fk_administratorius1) REFERENCES administratorius (id)
);

CREATE TABLE pirkejas
(
	age int,
	id int IDENTITY(1,1),,
	gimimodata date,
	vieta varchar (255),
	parduotuveskreditas float ,
	lytis int,
	fk_nuolaidoskodas int NOT NULL,
	PRIMARY KEY(id),
	UNIQUE(fk_nuolaidoskodas),
	FOREIGN KEY(lytis) REFERENCES lytis (id),
	CONSTRAINT priklauso FOREIGN KEY(fk_nuolaidoskodas) REFERENCES nuolaidoskodas (id),
	FOREIGN KEY(id) REFERENCES vartotojas (id)
);

CREATE TABLE produktas
(
	id int IDENTITY(1,1),
	pavadinimas varchar (255) NOT NULL,
	kaina float NOT NULL,
	aprasas VARCHAR(MAX)  NOT NULL,
	kiekis int NOT NULL,
	mase float NOT NULL,
	gamintojas varchar (255) NOT NULL,
	nuotraukos_url varchar (255) NOT NULL,
	fk_pardavėjas int NOT NULL,
	PRIMARY KEY(id),
	CONSTRAINT Sukuria FOREIGN KEY(fk_pardavėjas) REFERENCES pardavejas (id)
);

CREATE TABLE nuolaidoskodas_produktas
(
	minkiekis int,
	fk_produktas int NOT NULL,
	fk_nuolaidoskodas int NOT NULL,
	PRIMARY KEY(fk_nuolaidoskodas, fk_produktas),
	CONSTRAINT priklauso FOREIGN KEY(fk_produktas) REFERENCES produktas (id)
);

CREATE TABLE sandelisproduktas
(
	fk_sandelis int NOT NULL,
	fk_produktas int NOT NULL,
	PRIMARY KEY(fk_produktas, fk_sandelis),
	CONSTRAINT dalisJo FOREIGN KEY(fk_sandelis) REFERENCES sandelis (id),
	CONSTRAINT r FOREIGN KEY(fk_produktas) REFERENCES produktas (id)
);

CREATE TABLE uzsakymas
(
	kiekis int,
	verte float ,
	pradzia date,
	pabaiga date,
	siuntimoadresas varchar (255),
	id int IDENTITY(1,1),,
	fk_pirkejas int NOT NULL,
	PRIMARY KEY(id),
	CONSTRAINT Sukuria FOREIGN KEY(fk_pirkejas) REFERENCES pirkejas (id)
);

CREATE TABLE kategorijaProduktas
(
	fk_kategorija int NOT NULL,
	fk_produktas int NOT NULL,
	PRIMARY KEY(fk_kategorija, fk_produktas),
	CONSTRAINT Priklauso FOREIGN KEY(fk_kategorija) REFERENCES kategorija (id)
);

CREATE TABLE mokestis
(
	id int IDENTITY(1,1),
	count int,
	mokestis float ,
	fk_mokejimotipas int NOT NULL,
	fk_mokejimostatusas int NOT NULL,
	fk_pirkejas int NOT NULL,
	fk_uzsakymas int NOT NULL,
	fk_pirkejas1 int NOT NULL,
	PRIMARY KEY(id),
	UNIQUE(fk_mokejimotipas),
	UNIQUE(fk_mokejimostatusas),
	UNIQUE(fk_uzsakymas),
	FOREIGN KEY(fk_mokejimotipas) REFERENCES mokėjimotipas (id_mokėjimotipas),
	FOREIGN KEY(fk_mokejimostatusas) REFERENCES mokėjimostatusas (id),
	CONSTRAINT Gauna FOREIGN KEY(fk_pirkejas) REFERENCES pirkejas (id),
	CONSTRAINT sukuria FOREIGN KEY(fk_uzsakymas) REFERENCES uzsakymas (id),
	CONSTRAINT Moka FOREIGN KEY(fk_pirkejas1) REFERENCES pirkejas (id)
);

CREATE TABLE uzsakymasproduktas
(
	kaina float,
	fk_produktas int NOT NULL,
	fk_uzsakymas int NOT NULL,
	PRIMARY KEY(fk_produktas, fk_uzsakymas),
	CONSTRAINT prideda FOREIGN KEY(fk_produktas) REFERENCES produktas (id),
	CONSTRAINT duoda FOREIGN KEY(fk_uzsakymas) REFERENCES uzsakymas (id)
);