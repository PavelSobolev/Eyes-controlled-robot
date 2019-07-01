# Eyes-controlled-robot

##### The goal of this project is to design a desktop software (Windows) for providing users with eyes movements detection and tracking interface (with addition of brain-computer interface). 

[Video capture from the window of the application (Youtube)](https://youtu.be/QVo96w4uuyg)

The software focusses mainly on studying ways of implementing such interfaces in modern software systems. The following problems are planned to be explored:
* Hardware components, software components (operating systems, drivers, protocols, services) for creation and implementation of a robotized movable system.
* Application programming interfaces and software development kits and technologies for implementation of eyes movements detection and tracking interface (ETI),
* Application programming interfaces and software development kits and technologies for implementation of brain-computer interface (BCI),
* Development of software for utilising of APIs and SDKs of ETI and BCI for controlling of the robotized movable system. 

The software implements following functions:
* eyes movements detection and tracking interface for Microsoft Windows (in the form of dynamically linked libraries),
* brain-computer interface for Microsoft Windows (in the form of dynamically linked libraries),
* wireless network intercommunication between PC equipped with an eye tracker and BCI device and remote computer, which realizes functionality of robotized movable system (MS Windows, in the form of dynamically linked libraries).
* combining of ETI, BCI for driving a robot being implemented in this project (MS Windows, in the form of standard binary executable file).

##### Overall idea of design of software and hardware system can be represented by the following diagram. 

![App schema](https://github.com/PavelSobolev/Eyes-controlled-robot/blob/master/additional/principal.png)`
