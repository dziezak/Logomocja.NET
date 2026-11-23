# Poprawne przykłady
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

## Zmienne i operacje 
```
start {
	x <- 10
	y <- x * 2
	wartość <- losuj(x, y)
}
```


## Tablice
### Podstawowe odwołanie
```
start {
    mojatablica[0] <- 10
    x <- mojatablica[0] * 2
}
```

### Maksimum i odwracanie
```
oto maksimum(t, n) {
    m <- t[0]
    i <- 1
    powtórz(n - 1) {
        jeżeli (t[i] > m) {
            m <- t[i]
        }
        i <- i + 1
    }
    wynik m
}

oto odwróć(t, n) {
    i <- 0
    powtórz(n / 2) {
        tmp <- t[i]
        t[i] <- t[n - 1 - i]
        t[n - 1 - i] <- tmp
        i <- i + 1
    }
}

start {
    t <- tablica(6)
    i <- 0
    powtórz(6) {
        t[i] <- losuj(1, 100)
        i <- i + 1
    }
    pisz($"max = {maksimum(t, 6)}")
    odwróć(t, 6)
}
```

### Tablica dwuwymiarowa ( jeżeli będzie czas na implementację)

```
start {
    n <- 4
    m <- tablica(n)
    i <- 0
    powtórz(n) {
        m[i] <- tablica(n)
        j <- 0
        powtórz(n) {
            m[i][j] <- (i + 1) * (j + 1)
            j <- j + 1
        }
        i <- i + 1
    }
    pisz($"m[2][3] = {m[2][3]}")
    m[0][0] <- m[3][3] - m[1][1] * 2
}
```

## Funkcje

### Podstawowe formy 

```
start {
    x <- 7

    # bez inaczej #
    jeżeli (x > 5) {
        np(10)
    }

    # z inaczej #
    jeżeli (x % 2 = 0) {
        pisz("parzysta")
    } inaczej {
        pisz("nieparzysta")
    }

    # łańcuch #
    jeżeli (x < 0) {
        pisz("ujemna")
    } inaczej jeżeli (x = 0) {
        pisz("zero")
    } inaczej jeżeli (x < 10) {
        pisz("mała")
    } inaczej {
        pisz("duża")
    }
}
```

### Zagnieżdżone i puste bloki
```
start {
    a <- 3
    b <- 8
    jeżeli (a < b) {
        jeżeli (b > 5) {
            pisz("a < b i b > 5")
        } inaczej {
            pisz("a < b i b <= 5")
        }
    } inaczej {
        jeżeli (a = b) { }
    }
}
```

### Złożone warunki (nie, oraz, lub, nawiasy, ??, brak)
```
start {
    x <- 4
    y <- 10
    ż <- GetŻółw("Ala")

    jeżeli ((x > 1 lub y < 0) oraz nie (x = y)) { pw(90) }
    jeżeli (ż = brak) { pisz("brak żółwia") }
    jeżeli (ż?.nazwa() = "Ala") { pisz("to Ala") }
    jeżeli ((ż?.pozycja() ?? pos(0, 0)) = pos(0, 0)) { pisz("w punkcie startu") }
    jeżeli (nie prawda lub fałsz) { pisz("nigdy") }
}
```
### Złożone zagnieżdżenie wywołań wewnątrz argumentów funkcji
```
x <- losuj(mojatablica[dodaj(1, 2)], 100)
```
### Złożone warunki logiczne z wywołaniem funkcji i negacją
```
jeżeli (nie czy_gotowe() oraz x >= 0) { np(10) }
```


## program: Fraktalne drzewko
```
oto gałąź(długość, poziom) {
    jeżeli (poziom = 0) {
        wynik brak
    }
    np(długość)
    lw(30)
    gałąź(długość * 0.7, poziom - 1)
    pw(60)
    gałąź(długość * 0.7, poziom - 1)
    lw(30)
    cofnij(długość)
}

start {
    lw(90)
    gałąź(80, 6)
}
```

## interpolacja fizzbiz ( program z początku nauki )
```
oto fib(n) {
    jeżeli (n = 0 lub n = 1) {
        wynik n
    }
    wynik fib(n - 1) + fib(n - 2)
}

oto opis(i) {
    jeżeli (i % 15 = 0) {
        wynik "FizzBuzz"
    } inaczej jeżeli (i % 3 = 0) {
        wynik "Fizz"
    } inaczej jeżeli (i % 5 = 0) {
        wynik "Buzz"
    } inaczej {
        wynik $"{i}"
    }
}

start {
    i <- 1
    powtórz(15) {
        pisz($"{i}: {opis(i)}, fib({i}) = {fib(i)}")
        i <- i + 1
    }
}
```

## Wiele żółwi - Run

```
oto wielokąt(ż, boki, długość) {
    powtórz(boki) {
        ż.np(długość)
        ż.pw(360 / boki)
    }
}

start {
    ala <- żółw("Ala")
    bartek <- żółw("Bartek", pos(50, 0), 90, Kolor(255, 0, 0))

    wielokąt(ala, 5, 80)
    wielokąt(bartek, 3, 60)

    bartek.Run() {
        powtórz(8) {
            np(40)
            pw(45)
        }
    }
}
```

## Obsługa błędów

### Prosta obsługa błędów
```
start {
    spróbuj {
        pisz(10 / 0)
    } złap (e) {
        pisz("coś poszło nie tak")
    }
}
```

```
start {
    spróbuj {
        ż <- GetŻółw("Nieistniejący")
        ż.np(10)
    } złap (błąd) {
        pisz(błąd)
    }
}
```
### obsługa błędu
```
start {
    spróbuj {
        ż <- GetŻółw("Nieistniejący")
        ż.np(100)
    } złap (e) {
        pisz($"Nie udało się: {e}")
    }

    felek <- GetŻółw("Felek")
    felek?.np(10)

    cel <- felek?.pozycja() ?? pos(0, 0)
    kolor <- felek?.kolor() ?? Kolor(0, 0, 0)
    pisz($"cel = {cel}, kolor = {kolor}")
}
```
### Obsługa błędu z funkcją domyślą żółwia
```
oto bezpieczna_pozycja(imię) {
    ż <- GetŻółw(imię)
    wynik ż?.pozycja() ?? pos(0, 0)
}

start {
    p <- bezpieczna_pozycja("Ala")
    pisz($"Ala jest w {p}")

    nazwa_obcego <- GetŻółw("Bob")?.nazwa() ?? "nieznany"
    pisz(nazwa_obcego)
}
```

### Zagnieżdżenie spróbuj i minus po ??
```
oto podziel(a, b) {
    jeżeli (b = 0) {
        wynik brak
    }
    wynik a / b
}

start {
    spróbuj {
        spróbuj {
            t <- tablica(3)
            t[10] <- 1
        } złap (wewnętrzny) {
            pisz($"wewnętrzny błąd: {wewnętrzny}")
            ż <- GetŻółw("Nieistniejący")
            ż.np(10)
        }
    } złap (zewnętrzny) {
        pisz($"zewnętrzny błąd: {zewnętrzny}")
    }

    iloraz <- podziel(10, 0) ?? -1
    pisz($"iloraz = {iloraz}")
}
```


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

---

# Problemy 
## Ujemny indeks
```
x <- tablica[-1]
```

## Pusty argument w wywołaniu funkcji 
```
start {
    x <- max(10, )
}
```

## Pomylenie przypiania z poleceniem wywołania wewnątrz pętli 
```
start {
    x < - 10
}
```

## Brak dobrego korzystania z komentarzy 
```
start {
	# komentarz
	x <- 1
	pisz($"{x}")
}
```
