module ListExtensions =

    type System.Collections.Generic.List<'T> with

        // myMap
        member this.MyMap(func: 'T -> 'U) : 'U list =
            let rec map lst =
                match lst with
                | [] -> []
                | head :: tail -> func head :: map tail
            map (List.ofSeq this)

        // myFilter
        member this.MyFilter(func: 'T -> bool) : 'T list =
            let rec filter lst =
                match lst with
                | [] -> []
                | head :: tail ->
                    if func head then
                        head :: filter tail
                    else
                        filter tail
            filter (List.ofSeq this)

        member this.MySort(compare: 'T -> 'T -> bool) : 'T list =
            let rec insert x sortedList =
                match sortedList with
                | [] -> [x]
                | head :: tail ->
                    if compare x head then
                        x :: sortedList
                    else
                        head :: insert x tail
            let rec sort lst =
                match lst with
                | [] -> []
                | head :: tail -> insert head (sort tail)
            sort (List.ofSeq this)

        // Ascending Sort
        member this.MySortAscending() : 'T list =
            this.MySort(fun x y -> x < y)

        // Descending Sort
        member this.MySortDescending() : 'T list =
            this.MySort(fun x y -> x > y)