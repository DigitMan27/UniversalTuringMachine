About
=====

The Universal Turing Machine is an application written in *C\#(.NET 5)* in which you can simulate any turing machine .
(**Important Note:** This app supports only deterministic turing machines so make sure that your turing machine is deterministic. )
The app consists of three smaller “windows” which are :

-   The **Tape Data** which simulates the tape that the Turing Machine uses .

-   The **Simulated Turing Machine** which it shows the Turing Machine that the user has imported to the app.

-   And the **Execution Log** which shows the state of the simulation,whether your TM(Turing machine) has accepted or not the input of the tape and diagnostic messages as for example if the import of your TM was without problems.

The app accept only files with *.uTM*(universalTuringMachine) extension. The form of this file format is explained below.

File Format
===========

The format of the files with the *.uTM* extension must be exactly as mentioned or else the app will not function properly. The first line must always start with **States:q1,q2,q3,q4,...** in this line you declare the states of the Turing Machine(Note: the q1,q2,... can be whatever you want ex. States:a,b,c,dd,ca,ww,...). After the states declaration in the next line you must declare the accepting state and to do that write: **Accept:“your state”**. Afterwards you can use single line comments if need it (ex. // this is a comment) or leave empty lines or else write the description of your turing machine.
The description of your TM is the transitions between your states. The form of the description between two states, for example q1,q2 must be: *q1,“input”\(\rightarrow\)“write2Tape”,“HeaderMove”q2*.

-   “input” : Is the symbol that the Turing machine must read in the tape.

-   “write2Tape” : Is the symbol that the Turing machine will write to the tape. If you don’t want to write anything just ignore this. (ex. q1,a\(\rightarrow\)R,q2) .

-   “HeaderMove” : Shows the direction of the header. Accept the values ***L***: to move left , ***R***: to move right, ***N***: to stay still .

Examples
========

Below is shown an example of a Turing machine which diagnose the language

L = {2<sup>2N</sup> | N \> 0}.

The *.uTM* file that simulates the Turing machine of this language is:

|:--|
|States:q1,q2,q3,q4,q5,qy|
|Accept:qy|
||
|q1,0\(\rightarrow\)<span>ε</span>,R,q2|
|q2,x\(\rightarrow\)R,q2|
|q2,<span>ε</span>\(\rightarrow\)R,qy|
|q2,0\(\rightarrow\)x,R,q3|
|q3,0\(\rightarrow\)R,q4|
|q3,<span>ε</span>\(\rightarrow\)L,q5|
|q3,x\(\rightarrow\)R,q3|
|q4,x\(\rightarrow\)R,q4|
|q4,0\(\rightarrow\)x,R,q3|
|q5,0\(\rightarrow\)L,q5|
|q5,x\(\rightarrow\)L,q5|
|q5,<span>ε</span>\(\rightarrow\)R,q2|
||


