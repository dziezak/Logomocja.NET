## Podstawowy program
```
# Zwykła definicja procedury #
oto kwadrat (bok) {
    powtórz(4) {
        np(bok)
        pw(90)
    }
}

# Program właściwy #
start {
    kwadrat(100)
}
```
### Drzewo 
```
program
 ├── definicja_procedury
 │    ├── "oto"
 │    ├── identyfikator ("kwadrat")
 │    ├── lista_parametrów
 │    │    └── identyfikator ("bok")
 │    ├── blok
 │    │    ├── "{"
 │    │    ├── instrukcja -> pętla_powtórz
 │    │    │    ├── "powtórz"
 │    │    │    ├── wyrażenie -> alternatywa -> ... -> jednoargumentowy -> czynnik -> liczba (4)
 │    │    │    └── blok
 │    │    │         ├── "{"
 │    │    │         ├── instrukcja -> wywołanie_instrukcji
 │    │    │         │    ├── identyfikator ("np")
 │    │    │         │    └── lista_argumentów
 │    │    │         │         └── wyrażenie -> ... -> czynnik -> dostęp_do_pamięci -> identyfikator ("bok")
 │    │    │         └── "}"
 │    │    └── "}"
 │    └── "już"
 └── główny_blok
      ├── "start"
      └── blok
           ├── "{"
           ├── instrukcja -> wywołanie_instrukcji
           │    ├── identyfikator ("kwadrat")
           │    └── lista_argumentów
           │         └── wyrażenie -> ... -> czynnik -> liczba (100)
           └── "}"
```


## Zmienne i operacje 
```
start {
	x <- 10
	y <- x * 2
	wartość <- losuj(x, y)
}
```
### Drzewo y <- x * 2
```
instrukcja
 └── przypisanie
      ├── dostęp_do_pamięci
      │    └── identyfikator ("y")
      ├── "<-"
      └── wyrażenie
           └── alternatywa -> koniunkcja -> negacja -> porównanie -> suma
                ├── iloczyn
                │    ├── jednoargumentowy -> czynnik -> dostęp_do_pamięci -> identyfikator ("x")
                │    ├── "*"
                │    └── jednoargumentowy -> czynnik -> liczba (2)
```
### Drzewo wartość <- losuj(x, y)
```
instrukcja
 └── przypisanie
      ├── dostęp_do_pamięci
      │    └── identyfikator ("wartość")
      ├── "<-"
      └── wyrażenie -> alternatywa -> ... -> czynnik
           └── wywołanie_funkcji
                ├── identyfikator ("losuj")
                ├── "("
                ├── lista_argumentów
                │    ├── wyrażenie -> ... -> czynnik -> dostęp_do_pamięci -> identyfikator ("x")
                │    ├── ","
                │    └── wyrażenie -> ... -> czynnik -> dostęp_do_pamięci -> identyfikator ("y")
                └── ")"
```

## Odwołanie do tablic
```
start {
    mojatablica[0] <- 10
    x <- mojatablica[0] * 2
}
```

### Drzewo odwołania do tablic
```
instrukcja
 └── przypisanie
      ├── dostęp_do_pamięci
      │    ├── identyfikator ("mojatablica")
      │    ├── "["
      │    ├── wyrażenie -> ... -> czynnik -> liczba (0)
      │    └── "]"
      ├── "<-"
      └── wyrażenie -> ... -> czynnik -> liczba (10)
```
## Nowe odwołania do żółwi
```
start {
    nowy_żółw "Stefan"
    

    żółw("Stefan").podnies_pisak()
    żółw("Stefan").ustaw_kolor(255, 0, 0)
    

    żółw("Stefan").Run() {
        np(100)
        pw(90)
    }
}
```

### Drzewo ( nowy_żółw "Stefan")
```
instrukcja -> instrukcja_żółwia
 ├── "nowy_żółw"
 └── wyrażenie -> ... -> czynnik -> tekst ("Stefan")
```

### Drzewo (żółw("Stefan").podnies_pisak())
```
instrukcja -> instrukcja_żółwia
 ├── "żółw"
 ├── "("
 ├── wyrażenie -> ... -> czynnik -> tekst ("Stefan")
 ├── ")"
 ├── "."
 ├── identyfikator ("podnies_pisak")
 ├── "("
 ├── [ lista_argumentów: pusta ]
 └── ")"
```

### Drzewo (żółw("Stefan").Run() {np 100})
```
instrukcja -> instrukcja_żółwia
 ├── "żółw"
 ├── "("
 ├── wyrażenie -> ... -> czynnik -> tekst ("Stefan")
 ├── ")"
 ├── "."
 ├── "Run"
 ├── "("
 ├── ")"
 └── blok
      ├── "{"
      ├── instrukcja -> wywołanie_instrukcji
      │    ├── identyfikator ("np")
      │    └── lista_argumentów -> wyrażenie -> ... -> liczba (100)
      └── "}"
```

## Dynamiczna nazwa żółwia pobrana z tablicy
```
żółw(stafani_tablica[i]).podnies_pisak()
```
### Drzewo 
```
instrukcja -> instrukcja_żółwia
 ├── "żółw"
 ├── "("
 ├── wyrażenie -> ... -> czynnik -> dostęp_do_pamięci
 │    ├── identyfikator ("stafani_tablica")
 │    ├── "["
 │    ├── wyrażenie -> ... -> czynnik -> dostęp_do_pamięci -> identyfikator ("i")
 │    └── "]"
 ├── ")"
 ├── "."
 ├── identyfikator ("podnies_pisak")
 ├── "("
 └── ")"
```

## Złożone zagnieżdżenie wywołań wewnątrz argumentów funkcji
```
x <- losuj(mojatablica[dodaj(1, 2)], 100)
```
### Drzewo
```
instrukcja -> przypisanie
 ├── dostęp_do_pamięci ("x")
 ├── "<-"
 └── wyrażenie -> ... -> czynnik -> wywołanie_funkcji
      ├── identyfikator ("losuj")
      ├── "("
      ├── lista_argumentów
      │    ├── wyrażenie -> ... -> czynnik -> dostęp_do_pamięci
      │    │    ├── identyfikator ("mojatablica")
      │    │    ├── "["
      │    │    ├── wyrażenie -> ... -> czynnik -> wywołanie_funkcji
      │    │    │    ├── identyfikator ("dodaj")
      │    │    │    ├── "("
      │    │    │    ├── lista_argumentów (1, 2)
      │    │    │    └── ")"
      │    │    └── "]"
      │    ├── ","
      │    └── wyrażenie -> ... -> czynnik -> liczba (100)
      └── ")"
```


## Złożone warunki logiczne z wywołaniem funkcji i negacją
```
jeżeli (nie czy_gotowe() oraz x >= 0) { np(10) }
```
### Drzewo
```
instrukcja -> instrukcja_warunkowa
 ├── "jeżeli"
 ├── wyrażenie -> alternatywa -> koniunkcja
 │    ├── negacja
 │    │    ├── "nie"
 │    │    └── porównanie -> suma -> iloczyn -> jednoargumentowy -> czynnik -> wywołanie_funkcji ("czy_gotowe()")
 │    ├── "oraz"
 │    └── negacja (brak "nie") -> porównanie
 │         ├── suma -> ... -> dostęp_do_pamięci ("x")
 │         ├── operator_porównania (">=")
 │         └── suma -> ... -> liczba (0)
 └── blok
      ├── "{"
      ├── instrukcja -> wywołanie_instrukcji ("np 10")
      └── "}"
```


---

# Mini przykłady 

| Przykład                | Oczekiwany wynik           |
| ----------------------- | -------------------------- |
| x <- 1 + 2 * 3          | Poprawny                   |
| x <- -(1 + 2)           | Poprawny                   |
| x <- nie prawda         | Poprawny                   |
| x <- 10 < 20 oraz 5 > 2 | Poprawny                   |
| x <- f(g(h(1)))         | Poprawny                   |
| `x <- tab[a[b[c]]]`     | Poprawny                   |
| `x <- tab[1 + 2 * 3]`   | Poprawny                   |
| f()                     | Poprawne wywołanie funkcji |
| f(1,)                   | Błąd                       |
| f(,1)                   | Błąd                       |
| x <-                    | Błąd                       |
| jeżeli { }              | Błąd                       |
| powtórz { }             | Błąd                       |
| x <- 1 2                | Błąd                       |


# Problemy do rozwiązania przez interpreter
## Ujemny indeks
```
x <- tablica[-1]
```
### Drzewo
```
instrukcja -> przypisanie
 ├── dostęp_do_pamięci ("x")
 ├── "<-"
 └── wyrażenie -> ... -> czynnik -> dostęp_do_pamięci
      ├── identyfikator ("tablica")
      ├── "["
      ├── wyrażenie -> alternatywa -> ... -> iloczyn -> jednoargumentowy
      │    ├── "-"
      │    └── czynnik -> liczba (1)
      └── "]"
```
drzewo jest poprawne
Składnia radzi sobie z tym poprawnie na poziomie parsowania. Ponieważ zdefiniowaliśmy, że wewnątrz `[ ]` w `dostęp_do_pamięci` może znaleźć się dowolne `wyrażenie`, parser spokojnie "połknie" operator ujemny przed cyfrą. Pamiętaj jednak (na etapie interpretera), aby obsłużyć to jako błąd wykonania (indeksy w większości języków zaczynają się od 0, więc `-1` wyrzuci błąd).

---


## Pusty argument w wywołaniu funkcji 
```
start {
    x <- max(10, )
}
```

### Drzewo
```
instrukcja -> przypisanie
 ├── dostęp_do_pamięci ("x")
 ├── "<-"
 └── wyrażenie -> ... -> czynnik -> wywołanie_funkcji
      ├── identyfikator ("max")
      ├── "("
      ├── lista_argumentów
      │    ├── wyrażenie (czynnik -> liczba 10)
      │    ├── ","
      │    └── wyrażenie (BRAK - napotkano ")")
```

## Pomylenie przypiania z poleceniem wywołania wewnątrz pętli 
```
start {
    x < - 10
}
```

### Drzewo
```
instrukcja -> wywołanie_instrukcji
 ├── identyfikator ("x")
 └── [ lista_argumentów ]
      ├── wyrażenie (czyta to jako alternatywę logiczną...)
      │    └── porównanie
      │         ├── dostęp_do_pamięci -> identyfikator ("x" - odczytuje to, jakby x było zmienną, jeśli parser radzi sobie z wywołaniem w tym kontekście? NIE - patrz niżej)
```