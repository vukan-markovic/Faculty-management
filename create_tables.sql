------------------------drop-------------------
-----------------------------------------------------
--SEMA PORUKA
-----------------------------------------------------
if exists(select schema_id from sys.schemas where [name]='Poruka')
BEGIN
	if OBJECT_ID('[Poruka].[ZeljeneNotifikacije]','U') is not null
	BEGIN
		ALTER TABLE Poruka.ZeljeneNotifikacije DROP CONSTRAINT fk_zeljeneNotifikacije_student;
		ALTER TABLE Poruka.ZeljeneNotifikacije DROP CONSTRAINT fk_zeljeneNotifikacije_notifikacija;
		ALTER TABLE Poruka.ZeljeneNotifikacije DROP CONSTRAINT pk_zeljeneNotifikacije;
		DROP TABLE Poruka.ZeljeneNotifikacije;
	END
	if OBJECT_ID('[Poruka].[Dogadjaj]','U') is not null
	BEGIN
		ALTER TABLE Poruka.Dogadjaj DROP CONSTRAINT fk_dogadjaj_notifikacija;
		ALTER TABLE Poruka.Dogadjaj DROP CONSTRAINT pk_dogadjaj;
		DROP TABLE Poruka.Dogadjaj;
	END
	if OBJECT_ID('[Poruka].[StudentPorukaPrimalac]','U') is not null
	BEGIN
		ALTER TABLE Poruka.StudentPorukaPrimalac DROP CONSTRAINT fk_studentPorukaPrimalac_poruka;
		ALTER TABLE Poruka.StudentPorukaPrimalac DROP CONSTRAINT fk_studentPorukaPrimalac_student;
		ALTER TABLE Poruka.StudentPorukaPrimalac DROP CONSTRAINT pk_studentPorukaPrimalac;
		DROP TABLE Poruka.StudentPorukaPrimalac;
	END
	if OBJECT_ID('[Poruka].[Poruka]','U') is not null
	BEGIN
		ALTER TABLE Poruka.Poruka DROP CONSTRAINT fk_poruka_predavac;
		ALTER TABLE Poruka.Poruka DROP CONSTRAINT fk_poruka_tim;
		ALTER TABLE Poruka.Poruka DROP CONSTRAINT fk_poruka_student;
		ALTER TABLE Poruka.Poruka DROP CONSTRAINT fk_poruka_notifikacija;
		ALTER TABLE Poruka.Poruka DROP CONSTRAINT pk_poruka;
		DROP TABLE Poruka.Poruka;
	END
	if OBJECT_ID('[Poruka].[Notifikacija]','U') is not null
	BEGIN
		ALTER TABLE Poruka.Notifikacija DROP CONSTRAINT pk_notifikacija;
		DROP TABLE Poruka.Notifikacija;
	END
	DROP SCHEMA Poruka;
END
GO
------------------------------------------------------
--SEMA TIM
------------------------------------------------------
if exists (select schema_id from sys.schemas where [name] = 'Tim')
BEGIN
	if OBJECT_ID('[Tim].[StudentSastanak]','U') is not null
    BEGIN
		ALTER TABLE Tim.StudentSastanak DROP CONSTRAINT fk_studentSastanak_sastanak;
		ALTER TABLE Tim.StudentSastanak DROP CONSTRAINT fk_studentSastanak_student;
		ALTER TABLE Tim.StudentSastanak DROP CONSTRAINT pk_studentSastanak;
        DROP TABLE Tim.StudentSastanak;
    END
	if OBJECT_ID('[Tim].[Sastanak]','U') is not null
    BEGIN
		ALTER TABLE Tim.Sastanak DROP CONSTRAINT fk_sastanak_tim;
		ALTER TABLE Tim.Sastanak DROP CONSTRAINT pk_sastanak;
        DROP TABLE Tim.Sastanak;
    END
	if OBJECT_ID('[Tim].[StudentTim]','U') is not null
    BEGIN
		ALTER TABLE Tim.StudentTim DROP CONSTRAINT fk_studentTim_student;
		ALTER TABLE Tim.StudentTim DROP CONSTRAINT fk_studentTim_tim;
		ALTER TABLE Tim.StudentTim DROP CONSTRAINT pk_studentTim;
        DROP TABLE Tim.StudentTim;
    END
	if OBJECT_ID('[Tim].[PostavljeniFajlovi]','U') is not null
    BEGIN
		ALTER TABLE Tim.PostavljeniFajlovi DROP CONSTRAINT fk_postavljeniFajlovi_student;
		ALTER TABLE Tim.PostavljeniFajlovi DROP CONSTRAINT fk_postavljeniFajlovi_tim;
		ALTER TABLE Tim.PostavljeniFajlovi DROP CONSTRAINT pk_postavljeniFajlovi;
		DROP TABLE Tim.PostavljeniFajlovi;
    END
	if OBJECT_ID('[Tim].[Tim]','U') is not null
    BEGIN
		ALTER TABLE Tim.Tim DROP CONSTRAINT pk_tim;
        DROP TABLE Tim.Tim;
    END	
	DROP SCHEMA Tim;
END
GO
----------------------------------------
--SEMA STUDENT--------
---------------------------------------
if exists (select schema_ID from sys.schemas where [name] = 'Student')
BEGIN

	if OBJECT_ID('[Student].[StudentTest]','U') is not null
    BEGIN
		ALTER TABLE Student.StudentTest DROP CONSTRAINT pk_studentTest;
		ALTER TABLE Student.StudentTest DROP CONSTRAINT fk_studentTest_student;
		ALTER TABLE Student.StudentTest DROP CONSTRAINT fk_studentTest_test;
        DROP TABLE Student.StudentTest;
     END

	if OBJECT_ID('[Student].[Test]','U') is not null
    BEGIN
		ALTER TABLE Student.Test DROP CONSTRAINT pk_test;
		ALTER TABLE Student.Test DROP CONSTRAINT fk_test_kurs;
        DROP TABLE Student.Test;
     END
END
GO

-----------------------------------------------------
--SEMA KURS
-----------------------------------------------------
if exists (select schema_id from sys.schemas where [name] = 'Kurs')
BEGIN
	if OBJECT_ID('[Kurs].[PredavacKurs]','U') is not null
    BEGIN
		ALTER TABLE Kurs.PredavacKurs DROP CONSTRAINT fk_predavacKurs_kurs;
		ALTER TABLE Kurs.PredavacKurs DROP CONSTRAINT fk_predavacKurs_predavac;
		ALTER TABLE Kurs.PredavacKurs DROP CONSTRAINT pk_predavacKurs;
        DROP TABLE Kurs.PredavacKurs;
    END


	if OBJECT_ID('[Kurs].[Materijal]','U') is not null
    BEGIN
		ALTER TABLE Kurs.Materijal DROP CONSTRAINT fk_materijal_kurs;
		ALTER TABLE Kurs.Materijal DROP CONSTRAINT pk_materijal;
        DROP TABLE Kurs.Materijal;
    END

	if OBJECT_ID('[Kurs].[Kurs]','U') is not null
    BEGIN
		ALTER TABLE Kurs.Kurs DROP CONSTRAINT fk_kurs_predmet;
		ALTER TABLE Kurs.Kurs DROP CONSTRAINT pk_kurs;
        DROP TABLE Kurs.Kurs;
    END
	DROP SCHEMA Kurs;
END
GO

---------------------------------------------------
--SEMA PREDMET
---------------------------------------------------
if exists (select schema_id from sys.schemas where [name] = 'Predmet')
BEGIN
	if OBJECT_ID('[Predmet].[PohadjaniPredmet]','U') is not null
    BEGIN
		ALTER TABLE Predmet.PohadjaniPredmet DROP CONSTRAINT fk_pohadjaniPredmet_predmet;
		ALTER TABLE Predmet.PohadjaniPredmet DROP CONSTRAINT fk_pohadjaniPredmet_student;
		ALTER TABLE Predmet.PohadjaniPredmet DROP CONSTRAINT pk_pohadjaniPredmet;
        DROP TABLE Predmet.PohadjaniPredmet;
    END
	if OBJECT_ID('[Predmet].[Predmet]','U') is not null
    BEGIN
		ALTER TABLE Predmet.Predmet DROP CONSTRAINT fk_predmet_departman;
		ALTER TABLE Predmet.Predmet DROP CONSTRAINT pk_predmet;
        DROP TABLE Predmet.Predmet;
    END

	DROP SCHEMA Predmet;
END
GO
----------------------------------------
--SEMA STUDENT
---------------------------------------
if exists (select schema_ID from sys.schemas where [name] = 'Student')
BEGIN

	
	if OBJECT_ID('[Student].[Student]','U') is not null
    BEGIN
		ALTER TABLE Student.Student DROP CONSTRAINT fk_student_korisnik;
		ALTER TABLE Student.Student DROP CONSTRAINT pk_student;
        DROP TABLE Student.Student;
     END
	DROP SCHEMA Student;
END
GO

---------------------------------------------------
--SEMA PREDAVAC
---------------------------------------------------
if exists (select schema_ID from sys.schemas where [name] = 'Predavac')
BEGIN
	if OBJECT_ID('[Predavac].[Predavac]','U') is not null
    BEGIN
		ALTER TABLE Predavac.Predavac DROP CONSTRAINT fk_predavac_zvanjePredavaca;
		ALTER TABLE Predavac.Predavac DROP CONSTRAINT fk_predavac_korisnik;
		ALTER TABLE Predavac.Predavac DROP CONSTRAINT pk_predavac;
        DROP TABLE Predavac.Predavac;
     END
	 
	if OBJECT_ID('[Predavac].[ZvanjePredavaca]','U') is not null
    BEGIN
		ALTER TABLE Predavac.ZvanjePredavaca DROP CONSTRAINT pk_zvanjepredavaca;
        DROP TABLE Predavac.ZvanjePredavaca;
     END
	DROP SCHEMA Predavac;
END
GO

------------------------------------------------
--SEMA DEPARTMAN
------------------------------------------------
if exists (select schema_id from sys.schemas where [name] = 'Departman')
BEGIN
	if OBJECT_ID('[Departman].[Departman]','U') is not null
    BEGIN
		ALTER TABLE Departman.Departman DROP CONSTRAINT fk_departman_fakultet;
		ALTER TABLE Departman.Departman DROP CONSTRAINT pk_departman;
        DROP TABLE Departman.Departman;
    END
	if OBJECT_ID('[Departman].[Fakultet]','U') is not null
    BEGIN
		ALTER TABLE Departman.Fakultet DROP CONSTRAINT fk_fakultet_univerzitet;
		ALTER TABLE Departman.Fakultet DROP CONSTRAINT pk_fakultet;
        DROP TABLE Departman.Fakultet;
    END
	if OBJECT_ID('[Departman].[Univerzitet]','U') is not null
    BEGIN
		ALTER TABLE Departman.Univerzitet DROP CONSTRAINT pk_univerzitet;
        DROP TABLE Departman.Univerzitet;
    END
	DROP SCHEMA Departman;
END
GO
-----------------------------------------
-- SEMA KORISNIK
-----------------------------------------
if exists (select schema_ID from sys.schemas where [name] = 'Korisnik')
BEGIN
	if OBJECT_ID('[Korisnik].[Korisnik]','U') is not null
    BEGIN
		ALTER TABLE Korisnik.Korisnik DROP CONSTRAINT uq_korisnik_email;
		ALTER TABLE Korisnik.Korisnik DROP CONSTRAINT pk_korisnik;
        DROP TABLE Korisnik.Korisnik;
    END
	DROP SCHEMA Korisnik;
END
GO

-----------------------seme--------------------
CREATE SCHEMA Korisnik authorization dbo;
GO
CREATE SCHEMA Predavac authorization dbo;
GO
CREATE SCHEMA Departman authorization dbo;
GO
CREATE SCHEMA Predmet authorization dbo;
GO
CREATE SCHEMA Kurs authorization dbo;
GO
CREATE SCHEMA Student authorization dbo;
GO
CREATE SCHEMA Tim authorization dbo;
GO
CREATE SCHEMA Poruka authorization dbo;
go
-------------------------tabele
--tabela KORISNIK
CREATE TABLE Korisnik.Korisnik(
	KorisnikID int not null  IDENTITY(1,1),
	Username nvarchar(30) not null UNIQUE,
	Sifra nvarchar(20) not null,
	Ime nvarchar(20) not null,
	Prezime nvarchar(30) not null,
	Email nvarchar (30) not null,
	TrenutnaUloga nvarchar(30) not null,
	constraint pk_korisnik primary key (KorisnikID)
)
alter table Korisnik.Korisnik 
	add constraint uq_korisnik_email UNIQUE (Email);
go

--tabela UNIVERZITET
CREATE TABLE Departman.Univerzitet(
	univerzitetID int not null IDENTITY(1,1),
	NazivUniverziteta nvarchar(100) not null,
	constraint pk_univerzitet primary key (UniverzitetID)
)


--tabela FAKULTET
CREATE TABLE Departman.Fakultet(
	FakultetID int not null IDENTITY(1,1),
	NazivFakulteta nvarchar(100) not null,
	UniverzitetID int not null, --fk
	constraint pk_fakultet primary key(FakultetID)
)
alter table Departman.Fakultet
	add constraint fk_fakultet_univerzitet foreign key(UniverzitetID) references Departman.Univerzitet (UniverzitetID);
go

--tabela DEPARTMAN
CREATE TABLE Departman.Departman (
	DepartmanID int not null IDENTITY(1,1),
	NazivDepartmana nvarchar (100) not null,
	FakultetID int not null, --fk
	constraint pk_departman primary key (DepartmanID)
)
alter table Departman.Departman
	add constraint fk_departman_fakultet foreign key(FakultetID) references Departman.Fakultet (FakultetID);
go

--tabela ZVANJE PREDAVACA
CREATE TABLE Predavac.ZvanjePredavaca(
	ZvanjePredavacaID int not null  IDENTITY(1,1),
	NazivZvanjaPredavaca nvarchar(100) not null,
	constraint pk_zvanjepredavaca primary key (ZvanjePredavacaID)
)
 
--tabela PREDAVAC
CREATE TABLE Predavac.Predavac(
	PredavacID int not null  IDENTITY(1,1),
	DatumRodjenjaPredavaca date not null,
	MestoRodjenjaPredavaca nvarchar (20) not null,
	KatedraPredavaca nvarchar (50) not null,
	ZvanjePredavacaID int not null, --fk
 	KorisnikID int not null, --fk
	DepartmanID int not null, --fk
	constraint pk_predavac primary key(PredavacID)
)
alter table Predavac.Predavac
	add constraint fk_predavac_korisnik foreign key(KorisnikID) references Korisnik.Korisnik (KorisnikID);
go
alter table Predavac.Predavac
	add constraint fk_predavac_zvanjePredavaca foreign key(ZvanjePredavacaID) references Predavac.ZvanjePredavaca (ZvanjePredavacaID);
go
alter table Predavac.Predavac
	add constraint fk_predavac_departman foreign key(DepartmanID) references Departman.Departman (DepartmanID);


--tabela PREDMET
CREATE TABLE Predmet.Predmet(
	PredmetID int not null IDENTITY(1,1),
	NazivPredmeta nvarchar(100) not null,
	OznakaPredmeta nvarchar(10) not null unique,
	ECTSBodovi int not null,
	DepartmanID int not null, --fk
	Godina int not null,
	constraint pk_predmet primary key (PredmetID)
)
alter table Predmet.Predmet
	add constraint fk_predmet_departman foreign key(DepartmanID) references Departman.Departman (DepartmanID);
go


--tabela STUDENT
CREATE TABLE Student.Student(
	StudentID int not null  IDENTITY(1,1), 
	DatumRodjenjaStudenta date not null,
	MestoRodjenjaStudenta nvarchar (20) not null,
	BrojIndeksaStudenta nvarchar(10) not null unique,
	KatedraStudenta nvarchar(50) not null,
	StudijskiProgramStudenta nvarchar(100) not null,
	RbrUpisaneGodine int not null,
	GodinaUpisaFakulteta int not null,
	SifraStudenta nvarchar (30) not null unique,
	VrstaStudija nvarchar(100) not null,
	StepenStudija nvarchar(100) not null, 
	GodinaStudija nvarchar(100) not null, 
	NacinFinansiranja nvarchar(100) not null,
	KorisnikID int not null, --fk
	DepartmanID int not null, --fk
	constraint pk_student primary key (StudentID)
)
alter table Student.Student
	add constraint fk_student_korisnik foreign key(KorisnikID) references Korisnik.Korisnik (KorisnikID);

alter table Student.Student
	add constraint fk_student_departman foreign key(DepartmanID) references Departman.Departman (DepartmanID);

--tabela POHAÐANI PREDMET
CREATE TABLE Predmet.PohadjaniPredmet(
	PohadjaniPredmetID int not null IDENTITY(1,1),
	BrojBodova numeric (4) not null,
	PredmetID int not null, --fk
	Ocena int not null,
	StudentID int not null, --fk
	constraint pk_pohadjaniPredmet primary key (PohadjaniPredmetID),
	constraint chk_ocena check (Ocena in (5, 6, 7, 8, 9, 10))
)
alter table Predmet.PohadjaniPredmet
	add constraint fk_pohadjaniPredmet_predmet foreign key(PredmetID) references Predmet.Predmet (PredmetID);
go
alter table Predmet.PohadjaniPredmet
	add constraint fk_pohadjaniPredmet_student foreign key(StudentID) references Student.Student(StudentID);
go

--tabela KURS
CREATE TABLE Kurs.Kurs(
	KursID int not null IDENTITY(1,1),
	NazivKursa nvarchar(100) not null,
	BrStudenata numeric(4) not null,
	RasporedPredavanja nvarchar(100) not null,
	StudijskiProgram nvarchar(100) not null,
	SkolskaGodina nvarchar(10) not null, 
	TezinskiFaktorOcene decimal not null, 
	PolitikaUpisaOcena nvarchar(100) not null, 
	PolitikaUpisaDepartman nvarchar(100) not null, 
	PolitikaUpisaFCFS nvarchar(100) not null, 
	MinimalanBrStudenata numeric(4) not null,
	PredmetID int not null, --fk
	DefinicijaPreduslova nvarchar(100) not null,
	KriterijumZaPolaganje nvarchar (100) not null,
	Pravilo nvarchar(100) not null,
	constraint pk_kurs primary key (KursID)
)
alter table Kurs.Kurs
	add constraint fk_kurs_predmet foreign key(PredmetID) references Predmet.Predmet(PredmetID);
go
--tabela MATERIJAL
CREATE TABLE Kurs.Materijal(
	MaterijalID int not null IDENTITY(1,1),
	NazivMaterijala nvarchar(100) not null,
	KursID int not null,--fk
	constraint pk_materijal primary key (MaterijalID)
)
alter table Kurs.Materijal
	add constraint fk_materijal_kurs foreign key(KursID) references Kurs.Kurs (KursID);
go

--tabela PREDAVAC - KURS
CREATE TABLE Kurs.PredavacKurs(
	PredavacKursID int not null IDENTITY(1,1),
	KursID int not null, --fk 
	PredavacID int not null, --fk
	constraint pk_predavacKurs primary key (PredavacKursID)
)
alter table Kurs.PredavacKurs
	add constraint fk_predavacKurs_kurs foreign key(KursID) references Kurs.Kurs (KursID);
go
alter table Kurs.PredavacKurs
	add constraint fk_predavacKurs_predavac foreign key(PredavacID) references Predavac.Predavac(PredavacID);
go
--tabela TEST
CREATE TABLE Student.Test(
	TestID int not null IDENTITY(1,1),
    NazivTesta nvarchar(100) not null,
	KursID int not null,
	constraint pk_test primary key (TestID)
)
alter table Student.Test
	add constraint fk_test_kurs foreign key (KursID) references Kurs.Kurs(KursID);
go

--tabela STUDENT TEST
CREATE TABLE Student.StudentTest(
	StudentTestID int not null  IDENTITY(1,1),
	TestID int not null,
	StudentID int not null,
	constraint pk_studentTest primary key (StudentTestID)
)
alter table Student.StudentTest
	add constraint fk_studentTest_student foreign key (StudentID) references Student.Student(StudentID);
go
alter table Student.StudentTest
	add constraint fk_studentTest_test foreign key (TestID) references Student.Test(TestID);
go


--tabela TIM
CREATE TABLE Tim.Tim(
	TimID int not null IDENTITY(1,1),
	NazivTima nvarchar (30) not null,
	PredavacID int not null,--fk
	Ocena int null, 
	constraint pk_tim primary key (TimID),
	constraint chk_ocena check (Ocena in (5, 6, 7, 8, 9, 10))
)
alter table Tim.Tim
	add constraint fk_tim_predavac foreign key(PredavacID) references Predavac.Predavac(PredavacID);
go

--tabela POSTALJENI FAJLOVI
CREATE TABLE Tim.PostavljeniFajlovi(
	PostavljeniFajloviID int not null IDENTITY(1,1),
	NazivPostavljenogFajla nvarchar(100) not null,
	StudentID int null, --fk
	TimID int null, --fk
	constraint pk_postavljeniFajlovi primary key(PostavljeniFajloviID)
)
alter table Tim.PostavljeniFajlovi
	add constraint fk_postavljeniFajlovi_student foreign key(StudentID) references Student.Student(StudentID);
go
alter table Tim.PostavljeniFajlovi
	add constraint fk_postavljeniFajlovi_tim foreign key(TimID) references Tim.Tim (TimID);
go

--tabela STUDENT - TIM
CREATE TABLE Tim.StudentTim(
	StudentTimID int not null IDENTITY(1,1),
	TimID int not null, --fk
	StudentID int not null, --fk
	constraint pk_studentTim primary key(StudentTimID)
)
alter table Tim.StudentTim
	add constraint fk_studentTim_tim foreign key(TimID) references Tim.Tim (TimID);
go
alter table Tim.StudentTim
	add constraint fk_studentTim_student foreign key(StudentID) references Student.Student(StudentID);
go

--tabela SASTANAK
CREATE TABLE Tim.Sastanak(
	SastanakID int not null IDENTITY(1,1),
	TimID int not null, --fk
	PredavacID int not null, --fk
	VremeSastanka date not null, 
	MestoSastanka nvarchar(50) not null, 
	PovodSastanka nvarchar(200) not null,
	constraint pk_sastanak primary key (SastanakID)
)
alter table Tim.Sastanak
	add constraint fk_sastanak_tim foreign key(TimID) references Tim.Tim (TimID);

alter table Tim.Sastanak
	add constraint fk_sastanak_predavac foreign key(PredavacID) references Predavac.Predavac (PredavacID);
go

--tabela STUDENT - SASTANAK
CREATE TABLE Tim.StudentSastanak(
	StudentSastanakID int not null IDENTITY(1,1),
	SastanakID int not null, --fk
	StudentID int not null, --fk
	constraint pk_studentSastanak primary key(StudentSastanakID)
)
alter table Tim.StudentSastanak
	add constraint fk_studentSastanak_sastanak foreign key(SastanakID) references Tim.Sastanak (SastanakID);
go
alter table Tim.StudentSastanak
	add constraint fk_studentSastanak_student foreign key(StudentID) references Student.Student(StudentID);
go

--tabela Notifikacija

CREATE TABLE Poruka.Notifikacija(
	NotifikacijaID int not null IDENTITY(1,1),
	SadrzajNotifikacije nvarchar(500) not null,
	Ucestalost nvarchar(30) not null,
	constraint pk_notifikacija primary key(NotifikacijaID)
)

--tabela Poruka

CREATE TABLE Poruka.Poruka(
	PorukaID int not null IDENTITY(1,1),
	SadrzajPoruke nvarchar(1000),
	VrstaPoruke nvarchar (30) not null,
	VidljivostPoruke nvarchar(30) not null,
	PredavacID int null, --fk
	TimID int null, --fk
	StudentID INT null, --fk
	NotifikacijaID int not null, --fk
	constraint pk_poruka primary key(PorukaID)
)

alter table Poruka.Poruka
	add constraint fk_poruka_predavac foreign key(PredavacID) references Predavac.Predavac(PredavacID);
go
alter table Poruka.Poruka
	add constraint fk_poruka_tim foreign key(TimID) references Tim.Tim(TimID);
go
alter table Poruka.Poruka
	add constraint fk_poruka_student foreign key(StudentID) references Student.Student(StudentID);
go
alter table Poruka.Poruka
	add constraint fk_poruka_notifikacija foreign key(NotifikacijaID) references Poruka.Notifikacija(NotifikacijaID);
go
--tabela Student_Poruka_Primalac

CREATE TABLE Poruka.StudentPorukaPrimalac(
	StudentPorukaPrimalacID int not null IDENTITY(1,1),
	PorukaID int not null, --fk
	StudentID int not null, --fk
	constraint pk_studentPorukaPrimalac primary key(StudentPorukaPrimalacID)
)
alter table Poruka.StudentPorukaPrimalac
	add constraint fk_studentPorukaPrimalac_poruka foreign key(PorukaID) references Poruka.Poruka(PorukaID);
go
alter table Poruka.StudentPorukaPrimalac
	add constraint fk_studentPorukaPrimalac_student foreign key(StudentID) references Student.Student(StudentID);
go

--tabela Dogadjaj

CREATE TABLE Poruka.Dogadjaj(
	DogadjajID int not null IDENTITY(1,1),
	NazivDogadjaja nvarchar(30) not null,
	NotifikacijaID int not null, --fk 
	
	constraint pk_dogadjaj primary key(DogadjajID)
)
alter table Poruka.Dogadjaj
	add constraint fk_dogadjaj_notifikacija foreign key(NotifikacijaID) references Poruka.Dogadjaj(DogadjajID);
go
--tabela ZeljenaNotifikacija

CREATE TABLE Poruka.ZeljeneNotifikacije(
	ZeljeneNotifikacijeID int not null IDENTITY(1,1),
	StudentID int not null, --fk
	NotifikacijaID int not null, --fk 
	constraint pk_zeljeneNotifikacije primary key(ZeljeneNotifikacijeID)
)
alter table Poruka.ZeljeneNotifikacije
	add constraint fk_zeljeneNotifikacije_student foreign key(StudentID) references Student.Student(StudentID);
go
alter table Poruka.ZeljeneNotifikacije
	add constraint fk_zeljeneNotifikacije_notifikacija foreign key(NotifikacijaID) references Poruka.Notifikacija(NotifikacijaID);
go

