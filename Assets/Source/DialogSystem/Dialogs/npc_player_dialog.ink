INCLUDE dialog_var.ink


-> main
=== main ===
Phrase 1 #speaker:A
Phrase 2 #speaker:player
{option == "": -> before_choice | -> after_choice }
=== before_choice ===
Go make a choice #speker:A
-> END

=== after_choice ===
So ypu chose {option} #speaker:A
-> END