Insert into Korisnik.Korisnik values ('admin1','admin','Milan','Jovanovic','admin1','Admin');
Insert into Korisnik.Korisnik values ('admin2','admin','Milanka','Jaksic','admin2','Admin');
Insert into Korisnik.Korisnik values ('admin3','admin','Milojko','Jordan','admin3','Admin');
Insert into Korisnik.Korisnik values ('studnet1','student','Jovan','Jelic','student1','Student');
Insert into Korisnik.Korisnik values ('predavac1','predavac','Dusan','Jankovic','predavac1','Predavac');


Insert into Korisnik.Korisnik values ('aki','akiaki','Aleks','Peric','Alek@gmail.com','Predavac');
Insert into Korisnik.Korisnik values ('marko','markomarko','Marko','Kokic','marko@gmail.com','Predavac');
Insert into Korisnik.Korisnik values ('pera','perapera','Pera','Milicevic','pera@gmail.com','Predavac');
Insert into Korisnik.Korisnik values ('djura','djuradjura','Djuro','Sanovief','djura@gmail.com','Predavac');
Insert into Korisnik.Korisnik values ('sasa','sasasasa','Sasa','Stefanovic','sasa@gmail.com','Predavac');
Insert into Korisnik.Korisnik values ('dragan','dragandragan','Dragan','Dejic','dragan@gmail.com','Predavac');
Insert into Korisnik.Korisnik values ('alex','alexalex','Aleksandar','Petrovic','alex@gmail.com','Predavac');
Insert into Korisnik.Korisnik values ('ivan','ivanivan','Ivan','Macic','ivan@gmail.com','Predavac');
Insert into Korisnik.Korisnik values ('pedja','pedjapedja','Predrag','Komarica','pedja@gmail.com','Predavac');


Insert into Korisnik.Korisnik values ('maja','majamaja','Maja','Jelic','maja@gmail.com','Student');
Insert into Korisnik.Korisnik values ('miki','mikimiki','Milan','Delic','miki@gmail.com','Student');
Insert into Korisnik.Korisnik values ('sale','salesale','Sasa','Micic','sale@gmail.com','Student');
Insert into Korisnik.Korisnik values ('ajsa','ajsaajsa','Ajsa','Petrovic','ajsa@gmail.com','Student');
Insert into Korisnik.Korisnik values ('vuki','vukivuki','Vukan','Majic','vuki@gmail.com','Student');
Insert into Korisnik.Korisnik values ('deki','dekideki','Dejan','Gavrilovic','deki@gmail.com','Student');
Insert into Korisnik.Korisnik values ('saki','sakisaki','Sara','Maricic','saki@gmail.com','Student');


Insert into Departman.Univerzitet values ('UNS');
Insert into Departman.Univerzitet values ('UBG');
Insert into Departman.Univerzitet values ('UNIS');



Insert into Departman.Fakultet values ('Fakultet tehnickih nauka',1);
Insert into Departman.Fakultet values ('Prirodno matematicki fakultet',1);
Insert into Departman.Fakultet values ('Pravni fakultet',1);
Insert into Departman.Fakultet values ('ETF',2);



Insert into Departman.Departman values ('Elektrotehnika',1);
Insert into Departman.Departman values ('Menadzment',1);
Insert into Departman.Departman values ('Graficko inzenjersto',1);
Insert into Departman.Departman values ('Matematika',2);
Insert into Departman.Departman values ('Matematika',3);
Insert into Departman.Departman values ('Matematika',4);




Insert into Predavac.ZvanjePredavaca values ('Profesor');
Insert into Predavac.ZvanjePredavaca values ('Vanredni profesor');
Insert into Predavac.ZvanjePredavaca values ('Docent');
Insert into Predavac.ZvanjePredavaca values ('Asistent');
Insert into Predavac.ZvanjePredavaca values ('Demonstrator');




Insert into Predavac.Predavac values ('1956-12-12', 'Novi Sad','Komunikacioni sistemi' , 1, 6,1);
Insert into Predavac.Predavac values ('1975-12-12', 'Subotica','Komunikacioni sistemi' , 3, 7,1);
Insert into Predavac.Predavac values ('1976-12-12', 'Nis','Komunikacioni sistemi' , 2, 8,2);
Insert into Predavac.Predavac values ('1973-12-12', 'Novi Sad','Komunikacioni sistemi' , 2, 9,2);
Insert into Predavac.Predavac values ('1985-12-12', 'Beograd','Matematika' , 4, 10,4);
Insert into Predavac.Predavac values ('1993-12-12', 'Temerin','Dizajn sistemi' , 5, 11,3);
Insert into Predavac.Predavac values ('1965-12-12', 'Indjija','Komunikacioni sistemi' , 3, 12,1);
Insert into Predavac.Predavac values ('1963-12-12', 'Novi Sad','Komunikacioni sistemi' , 2, 13,2);
Insert into Predavac.Predavac values ('1986-12-12', 'Novi Sad','Dizajn sistemi' , 1, 14,3);




Insert into Predmet.Predmet values ('Elektronika','RJ434',5,1,1);
Insert into Predmet.Predmet values ('Fizika','GH4425',4,1,2);
Insert into Predmet.Predmet values ('Kvalitet','GF453',6,2,4);
Insert into Predmet.Predmet values ('Odnosi s korisnicima','UJ545664',4,2,2);
Insert into Predmet.Predmet values ('Projektovanje','HT353455',4,2,3);
Insert into Predmet.Predmet values ('Dizajn','YH356',5,3,1);
Insert into Predmet.Predmet values ('Stampa','UJ55654',5,3,4);
Insert into Predmet.Predmet values ('Ambalaza','AF445',6,3,3);
Insert into Predmet.Predmet values ('Statistika','KG56546',6,4,2);
Insert into Predmet.Predmet values ('Analiza','KGG5556',2,4,2);
Insert into Predmet.Predmet values ('Mikroprocesori','AF355',3,1,4);
Insert into Predmet.Predmet values ('Organizacioni sistemi','FD45435',3,2,3);
Insert into Predmet.Predmet values ('Preduzetnistvo','GFF454',6,2,3);
Insert into Predmet.Predmet values ('Uvod ET','TTYT455',5,1,2);


Insert into  Student.Student values ('1995-11-22','Nis','AG43','OPSTA','OSP',2,2016,'AG432016','OAS','IV','treca','budzet',15,1);
Insert into  Student.Student values ('1995-12-11','Beograd','AG45','OPSTA','OSP',2,2013,'AG452013','OAS','IV','druga','budzet',16,1);
Insert into  Student.Student values ('1995-05-12','Indjija','AG74','OPSTA','OSP',1,2017,'AG742017','OAS','IV','treca','budzet',17,1);
Insert into  Student.Student values ('1997-05-11','Vig','AH43','OPSTA','OSP',1,2015,'AH432015','OAS','prva','IV','budzet',18,2);
Insert into  Student.Student values ('1992-11-12','Palanka','KJ64','OPSTA','OSP',4,2013,'KJ642013','OAS','IV','cetvrta','samofinansiranje',19,2);
Insert into  Student.Student values ('1993-06-12','Ivanjica','OU64','OPSTA','OSP',2,2016,'OU642016','OAS','IV','druga','budzet',20,3);
Insert into  Student.Student values ('1994-02-11','Nis','AG44','JJ55','OSP',1,2016,'AG442016','OAS','prva','IV','budzet',21,3);




Insert into  Predmet.PohadjaniPredmet values (55,1,5,1);
Insert into  Predmet.PohadjaniPredmet values (63,2,7,1);
Insert into  Predmet.PohadjaniPredmet values (65,3,5,1);
Insert into  Predmet.PohadjaniPredmet values (75,4,8,1);
Insert into  Predmet.PohadjaniPredmet values (54,5,5,1);


Insert into  Predmet.PohadjaniPredmet values (64,6,7,2);
Insert into  Predmet.PohadjaniPredmet values (61,7,5,2);
Insert into  Predmet.PohadjaniPredmet values (66,8,7,2);
Insert into  Predmet.PohadjaniPredmet values (77,11,8,2);
Insert into  Predmet.PohadjaniPredmet values (72,12,5,2);


Insert into  Predmet.PohadjaniPredmet values (45,1,5,3);
Insert into  Predmet.PohadjaniPredmet values (62,2,7,3);
Insert into  Predmet.PohadjaniPredmet values (56,6,5,3);
Insert into  Predmet.PohadjaniPredmet values (44,7,5,3);


Insert into  Predmet.PohadjaniPredmet values (25,1,5,4);
Insert into  Predmet.PohadjaniPredmet values (45,7,5,4);
Insert into  Predmet.PohadjaniPredmet values (56,8,6,4);
Insert into  Predmet.PohadjaniPredmet values (55,12,6,4);
Insert into  Predmet.PohadjaniPredmet values (55,3,5,4);

Insert into  Predmet.PohadjaniPredmet values (25,3,5,6);
Insert into  Predmet.PohadjaniPredmet values (56,1,6,6);
Insert into  Predmet.PohadjaniPredmet values (15,4,5,6);


Insert into  Predmet.PohadjaniPredmet values (55,3,5,5);
Insert into  Predmet.PohadjaniPredmet values (65,12,7,5);
Insert into  Predmet.PohadjaniPredmet values (15,1,5,5);





Insert into Kurs.Kurs values ('NazivKursa',22,'Raspored','Studijski program','2018',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,1,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa2',12,'Raspored','Studijski program','2019',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,2,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa3',42,'Raspored','Studijski program','2019',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,3,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa',55,'Raspored','Studijski program','2019',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,4,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa2',21,'Raspored','Studijski program','2018',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,5,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa3',32,'Raspored','Studijski program','2018',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,6,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa',44,'Raspored','Studijski program','2018',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,7,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa2',25,'Raspored','Studijski program','2018',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,8,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa3',22,'Raspored','Studijski program','2019',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,9,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa',25,'Raspored','Studijski program','2019',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,10,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa2',44,'Raspored','Studijski program','2018',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,11,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa3',26,'Raspored','Studijski program','2019',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,12,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa',66,'Raspored','Studijski program','2018',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,13,'Preduslov','Kriterijum za polaganje','Pravilo');
Insert into Kurs.Kurs values ('NazivKursa2',12,'Raspored','Studijski program','2018',24,'Politika upisa ocena','Politika upisa departman','Politika upisa FCFS',5,14,'Preduslov','Kriterijum za polaganje','Pravilo');


Insert into Kurs.Materijal values ('Knjiga A',1);
Insert into Kurs.Materijal values ('Knjiga Arwer',2);
Insert into Kurs.Materijal values ('Knjiga Arrsdf',3);
Insert into Kurs.Materijal values ('Knjiga Afdfs',4);
Insert into Kurs.Materijal values ('Knjiga Aggg',5);
Insert into Kurs.Materijal values ('Knjiga FSA',6);
Insert into Kurs.Materijal values ('Knjiga AGSDG',7);
Insert into Kurs.Materijal values ('Knjiga GGGA',8);
Insert into Kurs.Materijal values ('Knjiga HDFHA',9);
Insert into Kurs.Materijal values ('Knjiga ADFHDF',10);
Insert into Kurs.Materijal values ('Knjiga AHDFH',11);
Insert into Kurs.Materijal values ('Knjiga HDHDA',12);



Insert into Kurs.Materijal values ('Projektno uputstvo',1);
Insert into Kurs.Materijal values ('Projektno uputstvo',2);
Insert into Kurs.Materijal values ('Projektno uputstvo',3);
Insert into Kurs.Materijal values ('Projektno uputstvo',4);
Insert into Kurs.Materijal values ('Projektno uputstvo',5);

Insert into Kurs.Materijal values ('Prezentacija',1);
Insert into Kurs.Materijal values ('Prezentacija',2);
Insert into Kurs.Materijal values ('Prezentacija',3);
Insert into Kurs.Materijal values ('Prezentacija',4);
Insert into Kurs.Materijal values ('Prezentacija',5);
Insert into Kurs.Materijal values ('Prezentacija',6);
Insert into Kurs.Materijal values ('Prezentacija',7);
Insert into Kurs.Materijal values ('Prezentacija',8);
Insert into Kurs.Materijal values ('Prezentacija',9);
Insert into Kurs.Materijal values ('Prezentacija',10);
Insert into Kurs.Materijal values ('Prezentacija',11);
Insert into Kurs.Materijal values ('Prezentacija',12);





Insert into  Kurs.PredavacKurs values (1,1);
Insert into  Kurs.PredavacKurs values (1,2);
Insert into  Kurs.PredavacKurs values (1,3);
Insert into  Kurs.PredavacKurs values (2,1);
Insert into  Kurs.PredavacKurs values (2,2);
Insert into  Kurs.PredavacKurs values (2,3);


Insert into  Kurs.PredavacKurs values (2,4);
Insert into  Kurs.PredavacKurs values (2,5);
Insert into  Kurs.PredavacKurs values (2,6);
Insert into  Kurs.PredavacKurs values (8,4);
Insert into  Kurs.PredavacKurs values (8,5);
Insert into  Kurs.PredavacKurs values (8,6);


Insert into  Kurs.PredavacKurs values (3,7);
Insert into  Kurs.PredavacKurs values (3,8);
Insert into  Kurs.PredavacKurs values (3,9);
Insert into  Kurs.PredavacKurs values (9,7);
Insert into  Kurs.PredavacKurs values (9,8);
Insert into  Kurs.PredavacKurs values (9,9);
--iuih


Insert into  Kurs.PredavacKurs values (4,1);
Insert into  Kurs.PredavacKurs values (4,9);
Insert into  Kurs.PredavacKurs values (4,2);
Insert into  Kurs.PredavacKurs values (5,3);
Insert into  Kurs.PredavacKurs values (5,4);

Insert into  Kurs.PredavacKurs values (7,1);
Insert into  Kurs.PredavacKurs values (7,3);
Insert into  Kurs.PredavacKurs values (7,4);



Insert into  Student.Test values ('Prvi test', 1);
Insert into  Student.Test values ('Prvi test', 1);
Insert into  Student.Test values ('Prvi test', 1);
Insert into  Student.Test values ('Prvi test', 2);
Insert into  Student.Test values ('Prvi test', 2);
Insert into  Student.Test values ('Drugi test', 2);
Insert into  Student.Test values ('Drugi test', 3);
Insert into  Student.Test values ('Drugi test', 3);
Insert into  Student.Test values ('Drugi test', 3);
Insert into  Student.Test values ('Drugi test', 4);

Insert into  Student.StudentTest values (1,1);
Insert into  Student.StudentTest values (2,1);
Insert into  Student.StudentTest values (3,1);
Insert into  Student.StudentTest values (4,1);
Insert into  Student.StudentTest values (5,1);
Insert into  Student.StudentTest values (1,2);
Insert into  Student.StudentTest values (2,2);
Insert into  Student.StudentTest values (3,2);
Insert into  Student.StudentTest values (4,2);
Insert into  Student.StudentTest values (5,2);


Insert into Tim.Tim values ('MODS',1,7);
Insert into Tim.Tim values ('GSF',2,8);
Insert into Tim.Tim values ('GFF',3,6);
Insert into Tim.Tim values ('SDS',4,7);
Insert into Tim.Tim values ('TRT',1,7);




Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, StudentID) values ('Fajl 231',1);
Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, StudentID) values ('Fajl 41',2);
Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, StudentID) values ('Fajl 121',1);
Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, StudentID) values ('Fajl gfg',1);
Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, StudentID) values ('Fajl abb',2);
Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, TimID) values ('Fajl 2fd',2);
Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, TimID) values ('Fajl fg',2);
Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, TimID) values ('Fajl aff',3);
Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, TimID) values ('Fajl fdg',3);

Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, StudentID) values ('Fajl g42',4);
Insert into Tim.PostavljeniFajlovi (NazivPostavljenogFajla, StudentID) values ('Fajl gfg1',4);




Insert into Tim.StudentTim (StudentID,TimID) values ( 1,1);
Insert into Tim.StudentTim  (StudentID,TimID) values ( 2,1);
Insert into Tim.StudentTim  (StudentID,TimID) values ( 5,1);
Insert into Tim.StudentTim (StudentID,TimID) values ( 6,1);
Insert into Tim.StudentTim (StudentID,TimID) values ( 1,2);
Insert into Tim.StudentTim (StudentID,TimID) values ( 4,2);
Insert into Tim.StudentTim (StudentID,TimID) values ( 2,2);
Insert into Tim.StudentTim (StudentID,TimID) values ( 5,3);
Insert into Tim.StudentTim (StudentID,TimID) values ( 6,3);
Insert into Tim.StudentTim (StudentID,TimID) values ( 6,4);
Insert into Tim.StudentTim (StudentID,TimID) values ( 4,4);


Insert into Tim.Sastanak values (1,1,'2019-12-22','Fakultet','Redovan');
Insert into Tim.Sastanak values (2,2,'2019-11-11','Fakultet','Redovan');
Insert into Tim.Sastanak values (3,3,'2019-11-23','Fakultet','Redovan');
Insert into Tim.Sastanak values (4,4,'2019-11-26','Fakultet','Redovan');
Insert into Tim.Sastanak values (5,1,'2019-11-14','Fakultet','Redovan');
Insert into Tim.Sastanak values (5,1,'2019-11-16','Fakultet','Vanredan');

Insert into Tim.StudentSastanak values (1,1);
Insert into Tim.StudentSastanak values (1,2);
Insert into Tim.StudentSastanak values (2,1);
Insert into Tim.StudentSastanak values (2,2);
Insert into Tim.StudentSastanak values (4,4);
Insert into Tim.StudentSastanak values (4,2);
Insert into Tim.StudentSastanak values (5,1);
Insert into Tim.StudentSastanak values (5,3);
Insert into Tim.StudentSastanak values (5,2);

	Insert into Poruka.Notifikacija values ('Sadrzaj notifikacije1' , 'Svaki dan');
	Insert into Poruka.Notifikacija values ('Sadrzaj notifikacije2' , 'Svaki dan');
	Insert into Poruka.Notifikacija values ('Sadrzaj notifikacije3' , 'Svaki dan');

  
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, PredavacID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',1, 1);
	Update Poruka.Poruka set TimID=1 where PorukaID=1
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, PredavacID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',2, 1);
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, PredavacID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',5, 2);
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, PredavacID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',1, 2);
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, PredavacID, NotifikacijaID) values ('Sadrzaj poruke' , 'Obavestenje','Vidljivo',2, 3);
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, PredavacID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',5, 3);
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, PredavacID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',1, 2);

	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, TimID, StudentID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',2,3, 1);
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, TimID, StudentID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',4,3, 2);
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, TimID, StudentID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',3,4, 1);
	Insert into Poruka.Poruka (SadrzajPoruke, VrstaPoruke, VidljivostPoruke, TimID, StudentID, NotifikacijaID) values ('Sadrzaj poruke' , 'Vazna poruka','Vidljivo',1,4, 3);
 

 	Insert into Poruka.Dogadjaj values ('Dogadjaj 1' , 1);
	Insert into Poruka.Dogadjaj values ('Dogadjaj 2' , 2);
 


  	Insert into Poruka.ZeljeneNotifikacije values (1,1);
	Insert into Poruka.ZeljeneNotifikacije values (2,2);
	Insert into Poruka.ZeljeneNotifikacije values (3,1);
	Insert into Poruka.ZeljeneNotifikacije values (4,1);


	