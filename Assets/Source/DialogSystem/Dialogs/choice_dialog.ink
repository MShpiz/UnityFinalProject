INCLUDE dialog_var.ink

{option == "": -> main | -> choice_made }
=== main ===
Phrase 1 #speaker:A
Phrase 2 #speaker:B
    + [option 1]
        ->chosen("option1")
    + [option 2]
        ->chosen("option2")
    + [option 3]
        ->chosen("option3")


=== chosen(option1) ===
you chose {option1} #speaker:A
-> END

=== choice_made ===
You already chose {option}!
-> END