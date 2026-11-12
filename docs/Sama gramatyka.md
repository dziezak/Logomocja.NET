
```
program                 = { definicja_procedury }, główny_blok;
główny_blok             = "start", blok;
definicja_procedury     = "oto" , identyfikator , lista_parametrów , blok;
lista_parametrów        = "(", [identyfikator , { ",", identyfikator } ], ")" ;
instrukcja              = przypisanie
						| instrukcja_warunkowa
						| pętla_powtórz
						| instrukcja_żółwia
						| powrót
						| wywołanie ;
						
przypisanie             = dostęp_do_pamięci, "<-", wyrażenie ;
dostęp_do_pamięci       = identyfikator , { "[", wyrażenie, "]" };

instrukcja_warunkowa    = "jeżeli" , "(" , wyrażenie, ")" , blok , [ "inaczej" , ( blok | instrukcja_warunkowa ) ] ;
pętla_powtórz           = "powtórz" ,  "(" , wyrażenie, ")" , blok ;

instrukcja_żółwia       = "nowy_żółw" , wyrażenie 
						| "żółw" , "(" , wyrażenie , ")" , "." , identyfikator , "(" , [ lista_argumentów ] , ")" 
						| "żółw" , "(" , wyrażenie , ")" , "." , "Run" , "(" , ")" , blok ;

powrót                  = "wynik" , wyrażenie ;
blok                    = "{" , { instrukcja } , "}" ;
lista_argumentów        = wyrażenie , { "," , wyrażenie } ;
```

```
wyrażenie               = alternatywa ;
alternatywa             = koniunkcja , { "lub" , koniunkcja } ;
koniunkcja              = negacja , { "oraz" , negacja } ;
negacja                 = ["nie"], porównanie ;
porównanie              = suma , [ operator_porównania , suma ] ;
suma                    = iloczyn , { ( "+" | "-" ) , iloczyn } ;
iloczyn                 = jednoargumentowy , { ( "*" | "/" | "%" ) , jednoargumentowy } ;
jednoargumentowy        = ["-"] , czynnik ;

czynnik                 = liczba
						| tekst
						| tekst_interpolowany
						| "prawda" | "fałsz"
						| dostęp_do_pamięci
						| wywołanie
						| "(" , wyrażenie , ")" ;

wywołanie               = identyfikator , "(" , [ lista_argumentów ], ")";

operator_porównania     = "=" | "<>" | "<" | ">" | "<=" | ">=" ;
```

```
litera                  = (* a-z, A-Z, ą ć ę ł ń ó ś ź ż oraz wielkie odpowiedniki *) ;
cyfra                   = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;
identyfikator           = litera , { litera | cyfra | "_" } ; (* poza słowami zastrzeżonymi *)
liczba                  = cyfra , { cyfra } , [ "." , cyfra , { cyfra } ] ;
znak_tekstu             = (* dowolny znak oprócz '"' i '\' *) ;
znak_interpolowany      = (* dowolny znak oprócz '"', '\', '{' i '}' *) ;
ucieczka                = "\" , ( '"' | "\" | "r" | "n" | "t" | "{" | "}" ) ;
tekst                   = '"' , { znak_tekstu | ucieczka } , '"' ;
tekst_interpolowany     = '$"' , { znak_interpolowany | ucieczka | "{" , wyrażenie , "}" } , '"' ;

komentarz               = "#" , { (* dowolny znak oprócz '#' *) } , "#" ;
biały_znak              = " " | "\t" | "\n" | "\r" ;
```

```
słowo_zastrzeżone = "start" | "oto" | "jeżeli" | "inaczej" | "powtórz" | "wynik" | "lub" | "oraz" | "nie" | "prawda" | "fałsz" | "nowy_żółw" | "żółw" |  "Run" ;
```



```
start {
	żółw("kornel").np(100)
	żółw "kornel"
	{
		np(100)
	}
}
```