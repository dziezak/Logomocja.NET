
```
program                 = { definicja_procedury }, główny_blok;
główny_blok             = "start", blok;
definicja_procedury     = "oto" , identyfikator ,"(" , lista_parametrów ,")", blok;
lista_parametrów        = [identyfikator , { ",", identyfikator } ] ;
instrukcja              = przypisanie
						| instrukcja_warunkowa
						| instrukcja_run
						| pętla_powtórz
						| instrukcja_próby
						| powrót
						| instrukcja_wywołania ;
						
przypisanie             = dostęp_do_pamięci, "<-", wyrażenie ;
dostęp_do_pamięci       = identyfikator , { "[", wyrażenie, "]" };

instrukcja_warunkowa    = "jeżeli" , "(" , wyrażenie, ")" , blok , [ "inaczej" , ( blok | instrukcja_warunkowa ) ] ;
pętla_powtórz           = "powtórz" ,  "(" , wyrażenie, ")" , blok ;
instrukcja_próby        = "spróbuj", blok, "złap", "(", identyfikator, ")", blok ;
instrukcja_wywołania    = wywołanie, { część_metody } | dostęp_do_pamięci, część_metody, { część_metody } ;

instrukcja_run          = ( wywołanie | dostęp_do_pamięci ), { część_metody }, ".", "Run", "(", ")", blok ;
część_metody            = ("." | "?." ), identyfikator, "(", [ lista_argumentów ], ")" ;

powrót                  = "wynik" , wyrażenie ;
blok                    = "{" , { instrukcja } , "}" ;
lista_argumentów        = wyrażenie , { "," , wyrażenie } ;
```

```
wyrażenie               = domyślna ;
domyślna                = alternatywa, { "??", alternatywa } ;
alternatywa             = koniunkcja , { "lub" , koniunkcja } ;
koniunkcja              = negacja , { "oraz" , negacja } ;
negacja                 = ["nie"], porównanie ;
porównanie              = suma , [ operator_porównania , suma ] ;
suma                    = iloczyn , { ( "+" | "-" ) , iloczyn } ;
iloczyn                 = jednoargumentowy , { ( "*" | "/" | "%" ) , jednoargumentowy } ;
jednoargumentowy        = ["-"] , czynnik ;

czynnik                 = podstawowy, {część_metody};
podstawowy              = liczba
						| tekst
						| tekst_interpolowany
						| "prawda" | "fałsz" | "brak"
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
słowo_zastrzeżone = "start" | "oto" | "jeżeli" | "inaczej" | "powtórz" | "wynik" | "lub" | "oraz" | "nie" | "prawda" | "fałsz" |  "Run" | "brak" | "spróbuj" | "złap" ;
```



```
start {
	a = żółw("Stefan")
	y = żółw("Felek", x)
	z = żółw("Felek1", pos(5, 6), heading, Kolor(0, 0, 0))
	
	spróbuj
	{
		x = GetŻółw("Stefan")
	}
	złap(e){
		pisz(e)
	}
	
	x.np(100)
	x.Run()
	{
		np(100)
	}
	
	k = 10
	pisz($"k = {k}")
}
```

