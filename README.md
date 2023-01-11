# Software and hardware system: movable robot controlled by user's gaze

##### The goal of this project is to plan, design and create a movable robot and a desktop software (Windows) for providing users with eyes movements detection interface (with addition of brain-computer interface) to control motion of the robot.

#### [Video capture from the window of the application (YouTube)](https://youtu.be/QVo96w4uuyg)
<!--#### [Presenation of the project (PPTX)](https://github.com/PavelSobolev/Eyes-controlled-robot/blob/master/additional/Capstone_Presenation_V4.pptx)-->

### Used technologies 
1. Operating systems: Windows, Linux
2. .NET Framework and third party .NET APIs
3. Visual C#
4. MS Visual Studio
5. Tobii Gaze API for .NET
6. NeuroSky MindWave Web REST API
7. Google Firebase (JSON)
8. NodeJS, Java Script
9. Secure Shell (SSH for Windows, Linux), SSH API for .NET
10. Python

### Hardware components 
1. Personal computer
2. Raspberry PI (3) computer
3. Components of the robot (Connectors, DC Motors and Motor Driver IC, Breadboar, Web camera)
4. Tobii eyes tracker
5. Mobile EEG (electroencephalograph)

### Description

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

### Used equipment 
#### Eyes tracking device
![hard](https://github.com/PavelSobolev/Eyes-controlled-robot/blob/master/additional/01.jpeg)
#### Mobile EEG (electroencephalograph)
![hard](https://github.com/PavelSobolev/Eyes-controlled-robot/blob/master/additional/02.jpeg)
#### Movable robot
![hard](https://github.com/PavelSobolev/Eyes-controlled-robot/blob/master/additional/03.png)

### User interface
![hard](https://github.com/PavelSobolev/Eyes-controlled-robot/blob/master/additional/04.png)
![hard](https://github.com/PavelSobolev/Eyes-controlled-robot/blob/master/additional/05.png)
![hard](https://github.com/PavelSobolev/Eyes-controlled-robot/blob/master/additional/06.png)
