# Optimization Methods

📘 Notatki do zaimplementowanych algorytmów

## 1. Cykl Eulera (Eulerian Cycle)
**Algorytm:** Hierholzer’s Algorithm

### ✅ Wymagania dla grafu:
- Graf **nieskierowany** – każda krawędź łączy dwa wierzchołki w obie strony
- Graf **spójny** – istnieje ścieżka między każdą parą wierzchołków (nie jest rozłączny)
- Każdy wierzchołek ma **parzysty stopień** – tzn. liczba wychodzących i wchodzących krawędzi jest parzysta

### 🧠 Opis:
Cykl Eulera to cykl, który przechodzi przez każdą krawędź grafu dokładnie raz i wraca do punktu początkowego.  
Algorytm Hierholzera konstruuje częściowe cykle i scala je w jeden pełny cykl Eulera.

### 📜 Kroki:
1. Wybierz wierzchołek startowy `u`
2. Buduj ścieżkę, dopóki są dostępne krawędzie, usuwając odwiedzone
3. Jeśli cykl się zamknął, ale są nieużyte krawędzie:
   - Wybierz nowy startowy wierzchołek z cyklu
   - Rozpocznij nowy cykl i połącz z poprzednim
4. Zakończ, gdy wszystkie krawędzie są odwiedzone

---

## 2. Maksymalne skojarzenie w grafie dwudzielnym
**Algorytm:** Algorytm ścieżek powiększających (Augmenting Path Algorithm, DFS)

### ✅ Wymagania:
- Graf **dwudzielny** – zbiór wierzchołków można podzielić na dwa rozłączne zbiory (V₁ i V₂), gdzie każda krawędź łączy wierzchołek z V₁ z wierzchołkiem z V₂
- Graf może być **skierowany lub nieskierowany**
- Krawędzie mogą być **nieważone**

### 🧠 Opis:
Skojarzenie to zbiór krawędzi, które nie mają wspólnych wierzchołków.  
Maksymalne skojarzenie to takie, którego nie da się już powiększyć bez konfliktu.  
Algorytm znajduje tzw. **ścieżki powiększające** i używa ich do rozszerzania skojarzenia.
Ścieżka powiększająca względem aktualnego skojarzenia to ścieżka w grafie,
która zaczyna się i kończy w wolnych wierzchołkach,
i której krawędzie naprzemiennie należą i nie należą do aktualnego skojarzenia.

### 📜 Kroki:
1. Zainicjuj `match[v] = -1` dla wszystkich wierzchołków
2. Dla każdego wolnego wierzchołka `u` z lewej partycji:
   - Uruchom DFS w celu znalezienia ścieżki powiększającej
   - Jeśli znaleziona, zaktualizuj skojarzenie
3. Powtarzaj aż brak ścieżek powiększających
   
---

## 3. Algorytm węgierski (Hungarian / Kuhn-Munkres Algorithm)
**Zastosowanie:** Minimalne skojarzenie kosztowe

### ✅ Wymagania:
- Graf **dwudzielny**, **skierowany**
- Liczba wierzchołków w obu partiach musi być taka sama (|V₁| = |V₂|)
- Wszystkie krawędzie mają **nieujemne wagi**
- Preferowany **graf pełny** – każdy wierzchołek z V₁ jest połączony z każdym z V₂

> 🔹 **Graf pełny** – zawiera krawędź między każdą możliwą parą wierzchołków z dwóch partycji  
> 🔹 **Graf skierowany** – krawędzie mają kierunek (u → v), tzn. przypisania są jednostronne

### 🧠 Opis:
Algorytm przypisuje elementy z V₁ do V₂ z minimalnym kosztem całkowitym.  
Działa na bazie **etykietowania wierzchołków** i budowania tzw. **grafu równości (Gl)**.  
Przy braku pełnego skojarzenia – algorytm aktualizuje etykiety wierzchołków i kontynuuje szukanie.

### 📜 Kroki:
1. Inicjalizacja etykiet:
   - `l(x) = max c(x, y)` dla x ∈ V₁
   - `l(y) = 0` dla y ∈ V₂
2. Buduj graf równości `Gl` (gdzie `l(x) + l(y) == c(x,y)`)
3. Znajdź pełne skojarzenie w `Gl`
4. Jeśli nie uda się:
   - Buduj drzewo naprzemienne (S, T)
   - Oblicz α = min `{ l(x) + l(y) - c(x,y) }` dla x ∈ S, y ∉ T
   - Zaktualizuj etykiety: `l(x) -= α`, `l(y) += α`
5. Powtarzaj aż znajdziesz pełne skojarzenie

---

## 4. Problem komiwojażera – metoda podziału i ograniczeń
**Algorytm:** Branch & Bound (Metoda podziału i ograniczeń)

### ✅ Wymagania:
- Graf **nieskierowany**
- Graf **spójny** – każda para wierzchołków jest połączona ścieżką
- Wszystkie wagi są **dodatnie**
- Preferowany **graf pełny** – każda para miast ma bezpośrednie połączenie

### 🧠 Opis:
Problem komiwojażera (TSP) polega na znalezieniu najkrótszego cyklu Hamiltona – trasy, która odwiedza wszystkie miasta dokładnie raz i wraca do punktu wyjścia.

Metoda podziału i ograniczeń:
- Przeszukuje tylko te części drzewa rozwiązań, które mają szansę na lepszy wynik
- Wykorzystuje *lower bounds* (dolne ograniczenia kosztu), aby odrzucać nieopłacalne ścieżki

### 📜 Kroki:
1. Zainicjuj `bestCost = ∞`
2. Wybierz wierzchołek startowy
3. Rekurencyjnie buduj trasę:
   - Dodaj sąsiada, jeśli nie był odwiedzony
   - Oblicz koszt częściowej trasy
   - Jeśli koszt ≥ `bestCost`, przytnij (nie kontynuuj)
   - W przeciwnym razie kontynuuj rekurencyjnie
4. Gdy odwiedzono wszystkie wierzchołki i wrócono do startu:
   - Jeśli koszt jest niższy niż `bestCost`, zapisz nową najlepszą trasę

---
