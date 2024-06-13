; *** Inno Setup version 6.1.0+ Latvian messages ***
;
; Translated from English by Zorgaats, zorgaats@gmail.com
;
; Note: When translating this text, do not add periods (.) to the end of
; messages that didn't have them already, because on those messages Inno
; Setup adds the periods automatically (appending a period would result in
; two periods being displayed).

[LangOptions]
LanguageName=Latviski
LanguageID=$0426
LanguageCodePage=1257

[Messages]

; *** Application titles
SetupAppTitle=Uzstādīšana
SetupWindowTitle=Uzstādīšana — %1
UninstallAppTitle=Noņemšana
UninstallAppFullTitle=Noņemšana — %

; *** Misc. common
InformationTitle=Informācija
ConfirmTitle=Apstiprināt
ErrorTitle=Kļūda

; *** SetupLdr messages
SetupLdrStartupMessage=Tiks uzstādīta programma %1 uz Jūsu datora. Vai vēlaties turpināt?
LdrCannotCreateTemp=Neiespējami izveidot pagaidu datnes. Uzstādīšana pārtraukta
LdrCannotExecTemp=Neiespējami palaist datni no pagaidu mapes. Uzstādīšana pārtraukta
HelpTextNote=

; *** Startup error messages
LastErrorMessage=%1.%n%nKļūda %2: %3
SetupFileMissing=Datne %1 nav atrodama uzstādīšanas mapē. Lūdzu izlabojiet kļūdu vai iegādājaties jaunu programmas kopiju.
SetupFileCorrupt=Uzstādīšanas datnes ir bojātas. Lūdzu iegādājaties jaunu programmas kopiju.
SetupFileCorruptOrWrongVer=Uzstādīšanas datnes ir bojātas vai nav savienojamas ar šo uzstādīšanas programmu. Lūdzu izlabojiet kļūdu vai iegādājaties jaunu programmas kopiju.
InvalidParameter=Komandrinda satur nepieļaujamu parametru:%n%n%1
SetupAlreadyRunning=Uzstādīšanas programma jau ir palaista.
WindowsVersionNotSupported=Šī programma neatbalsta Windows versiju, kas uzstādīta uz šī datora.
WindowsServicePackRequired=Programma pieprasa %1 Service Pack %2 vai jaunāku versiju.
NotOnThisPlatform=Šī pragramma nevar darboties uz %1.
OnlyOnThisPlatform=Programmu var palaist tikai uz %1.
OnlyOnTheseArchitectures=Programmu var uzstādīt tikai uz Windows versijas ar šādu procesoru arhitektūru:%n%n%1
WinVersionTooLowError=Programma pieprasa %1 versiju %2 vai jaunāku.
WinVersionTooHighError=Programmu nevar uzstādīt uz %1 versijas %2 vai jaunākas.
AdminPrivilegesRequired=Jums ir jābūt administratoram, lai varētu uzsākt uzstādīšanu.
PowerUserPrivilegesRequired=Jums ir jābūt administratoram vai pilnvarotam lietotājam, lai uzstādītu šo programmu.
SetupAppRunningError=Ir atrasts palaists eksemplārs %1.%n%nLūdzu,aizveriet visas programmas un spiediet "Ok" lai turpinātu vai "Atcelt", lai izietu.
UninstallAppRunningError=Noņemšana ir atklājusi, ka darbojas eksemplārs %1.%n%nLūdzu,aizveriet visas programmas un spiediet "Ok" lai turpinātu vai "Atcelt", lai izietu.

; *** Startup questions
PrivilegesRequiredOverrideTitle=Uzstādīšanas režīma izvēle
PrivilegesRequiredOverrideInstruction=Izvēlieties uzstādīšanas režīmu
PrivilegesRequiredOverrideText1=%1 var tikt uzstādīts vai nu visiem lietotājiem (nepieciešamas administratora privilēģijas), vai arī tikai Jums.
PrivilegesRequiredOverrideText2=%1 var tikt uzstādīts vai nu tikai Jums, vai arī visiem lietotājiem (nepieciešamas administratora privilēģijas).
PrivilegesRequiredOverrideAllUsers=Uzstādīt &visiem lietotājiem
PrivilegesRequiredOverrideAllUsersRecommended=Uzstādīt &visiem lietotājiem (rekomendējas)
PrivilegesRequiredOverrideCurrentUser=Uzstādīt tikai &man
PrivilegesRequiredOverrideCurrentUserRecommended=Uzstādīt tikai &man (rekomendējas)

; *** Misc. errors
ErrorCreatingDir=Nevar izveidot mapi "%1"
ErrorTooManyFilesInDir=Neiespējami izveidot datnes mapē "%1", jo tā satur pārāk daudz datņu

; *** Setup common messages
ExitSetupTitle=Iziet no uzstādīšanas
ExitSetupMessage=Uzstādīšana nav pabeigta. Ja Jūs tagad iziesiet, programma netiks uzstādīta.%n%nLai uzstādītu programmu, Jums būs atkal jāpalaiž uzstādīšana. %n%nIziet no uzstādīšanas?
AboutSetupMenuItem=&Par uzstādīšanu...
AboutSetupTitle=Par uzstādīšanu
AboutSetupMessage=%1, varsija %2%n%3%n%n%1mājas lapa:%n%4
AboutSetupNote=
TranslatorNote=Latvian translation by Zorgaats

; *** Buttons
ButtonBack=< &Atpakaļ
ButtonNext=&Tālāk >
ButtonInstall=&Uzstādīt
ButtonOK=OK
ButtonCancel=Atcelt
ButtonYes=&Jā
ButtonYesToAll=Jā &Visam
ButtonNo=&Nē
ButtonNoToAll=Nē V&isam
ButtonFinish=&Pabeigt
ButtonBrowse=Pā&rlūkot...
ButtonWizardBrowse=Pārlū&kot...
ButtonNewFolder=I&zveidot jaunu mapi

; *** "Select Language" dialog messages
SelectLanguageTitle=Izvēlieties uzstādīšanas valodu
SelectLanguageLabel=Izvēlieties valodu, kurā notiks uzstādīšana:

; *** Common wizard text
ClickNext=Spiediet "Tālāk", lai turpinātu, vai "Atcelt", lai izietu no uzstādīšanas.
BeveledLabel=
BrowseDialogTitle=Pārlūkot mapi
BrowseDialogLabel=Izvēlieties mapi no saraksta, tad spiediet "Ok".
NewFolderName=Jauna mape

; *** "Welcome" wizard page
WelcomeLabel1=Вас приветствует Мастер установки [name]
WelcomeLabel2=Programma uzstādīs [name/ver] uz Jūsu datora.%n%nPirms uzstādīšanas vēlams aizvērt visas programmas.

; *** "Password" wizard page
WizardPassword=Parole
PasswordLabel1=Uzstādīšana ir aizsargāta ar paroli.
PasswordLabel3=Lūdzu, ievadiet paroli, tad spiediet "Tālāk", lai turpinātu. Parole ir reģistrjūtīga.
PasswordEditLabel=&Parole:
IncorrectPassword=Jūsu ievadītā parole ir nepareiza. Lūdzu, mēģiniet vēlreiz.

; *** "License Agreement" wizard page
WizardLicense=Licence
LicenseLabel=Lūdzu, izlasiet sekojošo informāciju, pirms turpināt.
LicenseLabel3=Lūdzu, izlasiet Līgumu. Jums ir jāapstiprina Līgums, lai turpinātu uzstādīšanu.
LicenseAccepted=Es &piekrītu līgumam
LicenseNotAccepted=Es &nepiekrītu līgumam

; *** "Information" wizard pages
WizardInfoBefore=Informācija
InfoBeforeLabel=Lūdzu, izlasiet šo informāciju.
InfoBeforeClickLabel=Kad esat gatavs turpināt uzstādīšanu, spiediet "Tālāk".
WizardInfoAfter=Informācija
InfoAfterLabel=Lūdzu, izlasiet šo informāciju.
InfoAfterClickLabel=Kad esat gatavs turpināt uzstādīšanu, spiediet "Tālāk".

; *** "User Information" wizard page
WizardUserInfo=Lietotāja informācija
UserInfoDesc=Lūdzu, ievadiet datus par sevi.
UserInfoName=&Lietotāja vārds:
UserInfoOrg=&Organizācija:
UserInfoSerial=&Sērijas numurs:
UserInfoNameRequired=Jums ir jāievada savs vārds.

; *** "Select Destination Location" wizard page
WizardSelectDir=Uzstādīšanas mapes izvēle
SelectDirDesc=Kur [name] tiks instalēts?
SelectDirLabel3=[name] datnes tiks instalētas norādītajā mapē.
SelectDirBrowseLabel=Lai turpinātu, spiediet "Tālāk". Ja vēlaties norādīt citu mapi, spiediet "Pārlūkot".
DiskSpaceGBLabel=Ir nepieciešami brīvi [gb] GB uz cietā diska.
DiskSpaceMBLabel=Ir nepieciešami brīvi [mb] MB uz cietā diska.
CannotInstallToNetworkDrive=Uzstādīšana nevar tikt veikta uz tīkla diska.
CannotInstallToUNCPath=Uzstādīšana nevar tikt veikta mapē pa UNC-adresi.
InvalidPath=Jums ir jānorāda pilna uzstādīšanas adrese, piemērs:%n%nC:\APP%n%nvai UNC adrese:%n%n\\server\share
InvalidDrive=Disks vai tīkla adrese, kuru Jūs izvēlējāties, nepastāv vai arī nav pieejams. Lūdzu, izvēlieties citu.
DiskSpaceWarningTitle=Nepietiek vietas uz diska
DiskSpaceWarning=Uzstādīšanai ir nepieciešami vismaz %1 KB brīvas vietas uz diska, bet pieejami ir tikai %2 KB.%n%nVai vēlaties turpināt?
DirNameTooLong=Mapes nosaukums vai adrese ir pārāk gara.
InvalidDirName=Mapes nosaukums nav derīgs.
BadDirName32=Mapes nosaukumā nedrīkst būt šādi simboli: %n%n%1
DirExistsTitle=Mape jau pastāv.
DirExists=Mape:%n%n%1%n%njau pastāv. Vai vienalga vēlaties turpināt?
DirDoesntExistTitle=Mape nepastāv
DirDoesntExist=Mape%n%n%1%n%nnepastāv. Vai vēlaties to izveidot?

; *** "Select Components" wizard page
WizardSelectComponents=Izvēlieties sastāvdaļas
SelectComponentsDesc=Kurus komponentus vēlaties uzstādīt?
SelectComponentsLabel2=Izvēlieties komponentus, kurus vēlaties uzstādīt. Spiediet "Tālāk", lai turpinātu.
FullInstallation=Pilna uzstādīšana
; if possible don't translate 'Compact' as 'Minimal' (I mean 'Minimal' in your language)
CompactInstallation=Kompakta uzstādīšana
CustomInstallation=Izveidot uzstādīšanu
NoUninstallWarningTitle=Komponenti jau pastāv
NoUninstallWarning=Uzstādīšana ir atklājusi, ka šādi komponenti jau ir uzstādīti:%n%n%1%n%nŠo komponentu uzstādīšanas atcelšana neizdzēsīs tos.%n%nVai turpināt?
ComponentSize1=%1 Кб
ComponentSize2=%1 Мб
ComponentsDiskSpaceGBLabel=Pašlaik izvēlētie komponenti aizņem [gb] GB uz cietā diska.
ComponentsDiskSpaceMBLabel=Pašlaik izvēlētie komponenti aizņem [mb] MB uz cietā diska.

; *** "Select Additional Tasks" wizard page
WizardSelectTasks=Papilduzdevumu izvēlne
SelectTasksDesc=Kurus papilduzdevumus vajadzētu veikt?
SelectTasksLabel2=Izvēlieties, kādi papilduzdevumi tiks veikti [name] uzstādīšanas laikā, tad spiediet "Tālāk".

; *** "Select Start Menu Folder" wizard page
WizardSelectProgramGroup=Izvēlieties Start Menu mapi
SelectStartMenuFolderDesc=Kur uzstādīšanas programmai vajadzētu likt īsinājumikonas?
SelectStartMenuFolderLabel3=Uzstādīšana izveidos īsinājumikonas Start Menu mapē.
SelectStartMenuFolderBrowseLabel=Lai turpinātu, spiediet "Tālāk". Ja vēlaties norādīt citu mapi, spiediet "Pārlūkot".
MustEnterGroupName=Jums ir jānorāda mape.
GroupNameTooLong=Mapes nosaukums ir pārāk garš.
InvalidGroupName=Mape nav derīga.
BadGroupName=Mapes nosaukums satur kādu no šiem simboliem:%n%n%1
NoProgramGroupCheck2=&Neizveidot Start Menu mapi

; *** "Ready to Install" wizard page
WizardReady=Gatavs uzstādīšanai
ReadyLabel1=Uzstādīšana ir gatava instalēt [name] uz Jūsu datora.
ReadyLabel2a=Spiediet "Uzstādīt", lai sāktu uzstādīšanu, vai spiediet "Atpakaļ", lai izmainītu parametrus.
ReadyLabel2b=Spiediet "Uzstādīt", lai sāktu uzstādīšanu.
ReadyMemoUserInfo=Lietotāja informācija:
ReadyMemoDir=Galamēķis:
ReadyMemoType=Uzstādīšanas tips:
ReadyMemoComponents=Izvēlētie komponenti:
ReadyMemoGroup=Start Menu mape:
ReadyMemoTasks=Papilduzdevumi:

; *** TDownloadWizardPage wizard page and DownloadTemporaryFile
DownloadingLabel=Papildus datņu lejupielāde...
ButtonStopDownload=&Pārtraukt ielādi
StopDownload=Jūs tiešām vēlaties pārtraukt lejupielādi?
ErrorDownloadAborted=Lejupielāde pārtraukta
ErrorDownloadFailed=Lejupielādes kļūda: %1 %2
ErrorDownloadSizeFailed=Izmēra kļūda: %1 %2
ErrorFileHash1=Ошибка хэша файла: %1
ErrorFileHash2=Неверный хэш файла: ожидался %1, получен %2
ErrorProgress=Izpildes kļūda: %1 из %2
ErrorFileSize=Kļūdains faila izmērs: tika gaidīts %1, iegūts %2

; *** "Preparing to Install" wizard page
WizardPreparing=Gatavoties uzstādīšanai
PreparingDesc=Uzstādīšana ir gatava instalēt [name] uz Jūsu datora.
PreviousInstallNotCompleted=Uzstādīšana/noņemšana iepriekšējai programmai nav pabeigta. Jums ir jāpārstartē dators, lai pabeigtu uzstādīšanu.%n%nPēc pārstartēšanas palaidiet uzstādīšanu no jauna, lai pabeigtu uzstādīt [name].
CannotContinue=Uzstādīšanu nevar turpināt. Lūdzu, spiediet "Atcelt", lai izietu.
ApplicationsFound=Sekojošas programmas izmanto datnes, kuras uzstādīšanai jāatjauno. Rekomendējas uzstādīšanai atļaut automātiski aizvērt šīs programmas.
ApplicationsFound2=Sekojošas programmas izmanto datnes, kuras uzstādīšanai jāatjauno. Rekomendējas uzstādīšanai atļaut automātiski aizvērt šīs programmas. Kad instalācija būs pabeigta, uzstādīšana mēģinās tās atkal palaist.
CloseApplications=&Automātiski aizvērt šīs programmas
DontCloseApplications=&Neaizvērt šīs programmas
ErrorCloseApplications=Uzstādīšanai neizdevās automātiski aizvērt visas programmas.Pirms uzstādīšanas rekomendējas aizvērt visas programmas, kas izmanto atjaunināmās datnes.
PrepareToInstallNeedsRestart=Uzstādīšanai nepieciešams pārstartēt Jūsu datoru. Kad dators pārstartēsies, lūdzu, palaidiet uzstādīšanas programmu vēlreiz, lai pabeigtu uzstādīšanu [name].%n%nVeikt pārstartēšanu tūlīt?

; *** "Installing" wizard page
WizardInstalling=Uzstādīšana...
InstallingLabel=Lūdzu, uzgaidiet, kamēr [name] tiks uzstādīts uz Jūsu datora.

; *** "Setup Completed" wizard page
FinishedHeadingLabel=Pabeigta [name] uzstādīšana
FinishedLabelNoIcons=Uzstādīšana pabeigta.
FinishedLabel=Programma [name] ir uzstādīta uz Jūsu datora. Programmu var palaist, uzklikšķinot uz izveidotajām ikonām.
ClickFinish=Spiediet "Pabeigt", lai aizvērtu uzstādīšanu.
FinishedRestartLabel=Lai pabeigtu [name] uzstādīšanu, nepieciešams pārstartēt Jūsu datoru. Vai vēlaties to darīt tagad?
FinishedRestartMessage=Lai pabeigtu [name] uzstādīšanu, nepieciešams pārstartēt Jūsu datoru.%n%nVai vēlaties to darīt tagad?
ShowReadmeCheck=Jā, vēlos apskatīt README failu
YesRadio=&Jā, pārstartēt datoru tagad
NoRadio=&Nē, datoru pārstartēšu vēlāk
; used for example as 'Run MyProg.exe'
RunEntryExec=Palaist %1
; used for example as 'View Readme.txt'
RunEntryShellExec=Apskatīt %1

; *** "Setup Needs the Next Disk" stuff
ChangeDiskTitle=Uzstādīšanai ir nepieciešams nākamais disks
SelectDiskLabel2=Lūdzu, ielieciet %1 disku un spiediet "Ok".%n%nJa datne ir atrodama uz šī paša diska kādā citā mapē, norādiet tās atrašanās vietu vai spiediet "Pārlūkot", lai to norādītu.
PathLabel=&Ceļš:
FileNotInDir2=Datne "%1" neatrodas "%2". Lūdzu, ielieciet pareizo disku vai norādiet pareizo mapi.
SelectDirectoryLabel=Lūdzu, norādiet nākamā diska atrašanās vietu.

; *** Installation phase messages
SetupAborted=Uzstādīšana netika pabeigta.%n%nLūdzu, izlabojiet kļūdu un palaidiet uzstādīšanu no jauna.
AbortRetryIgnoreSelectAction=Izvēlieties darbību
AbortRetryIgnoreRetry=Mēģināt no &jauna
AbortRetryIgnoreIgnore=&Ignorēt kļūdu un turpināt
AbortRetryIgnoreCancel=Pārtraukt uzstādīšanu

; *** Installation status messages
StatusClosingApplications=Programmu aizvēršana...
StatusCreateDirs=Mapju izveidošana...
StatusExtractFiles=Datņu kopēšana...
StatusCreateIcons=Īsinājumikonu izveidošana...
StatusCreateIniEntries=Izveido INI ierakstu...
StatusCreateRegistryEntries=Izveido reģistra ierakstus...
StatusRegisterFiles=Reģistrē datnes...
StatusSavingUninstall=Saglabā noņemšanas datus...
StatusRunProgram=Pabeidz uzstādīšanu...
StatusRestartingApplications=Programmu restartēšana...
StatusRollback=Izmaiņu atiestatīšana...

; *** Misc. errors
ErrorInternal2=Iekšēja kļūda: %1
ErrorFunctionFailedNoCode=%1: cieta neveiksmi
ErrorFunctionFailed=%1: cieta neveiksmi; kods %2
ErrorFunctionFailedWithMessage=%1: cieta neveiksmi; kods %2.%n%3
ErrorExecutingProgram=Nespēju palaist failu:%n%1

; *** Registry errors
ErrorRegOpenKey=Kļūda, atverot reģistra atslēgu:%n%1\%2
ErrorRegCreateKey=Kļūda, izveidojot reģistra atslēgu:%n%1\%2
ErrorRegWriteKey=Kļūda, rakstot reģistra atslēgu:%n%1\%2

; *** INI errors
ErrorIniEntry=Kļūda, izveidojot INI ieraksta datni "%1".

; *** File copying errors
FileAbortRetryIgnoreSkipNotRecommended=I&zlaist šo failu (nerekomendējas)
FileAbortRetryIgnoreIgnoreNotRecommended=&Ignorēt kļūdu un turpināt (nerekomendējas)
SourceIsCorrupted=Datnes avots ir bojāts
SourceDoesntExist=Datnes avots "%1" nepastāv
ExistingFileReadOnly2=Nevar aizstāt esošo failu, tā kā tas ir iezīmēts kā "read only".
ExistingFileReadOnlyRetry=&Dzēst atribūtu "read only" un atkārtot mēģinājumu
ExistingFileReadOnlyKeepExisting=&Paturēt esošo failu
ErrorReadingExistingDest=Kļūda, mēģinot lasīt pastāvošo failu:
FileExistsSelectAction=Izvēlieties darbību
FileExists2=Fails jau pastāv.
FileExistsOverwriteExisting=&Aizstāt esošo failu
FileExistsKeepExisting=&Saglabāt esošo failu
FileExistsOverwriteOrKeepAll=A&tkārtot darbību visiem turpmākajiem konfliktiem
ExistingFileNewerSelectAction=Izvēlieties darbību
ExistingFileNewer2=Esošais fails ir jaunāks nekā uzstādāmais.
ExistingFileNewerOverwriteExisting=&Aizstāt esošo failu
ExistingFileNewerKeepExisting=&Saglabāt esošo failu (rekomendējas)
ExistingFileNewerOverwriteOrKeepAll=A&tkārtot darbību visiem turpmākajiem konfliktiem
ErrorChangingAttr=Radusies kļūda, mēģinot nomainīt datnes īpašību:
ErrorCreatingTemp=Radusies kļūda, izveidojot datni galamērķa mapē:
ErrorReadingSource=Radusies kļūda, nolasot datni:
ErrorCopying=Radusies kļūda, pārkopējot datni:
ErrorReplacingExistingFile=Radusies kļūda, pārrakstot jau pastāvošo datni:
ErrorRestartReplace=Atkārtota aizstāšana cietusi neveiksmi:
ErrorRenamingTemp=Radusies kļūda, nomainot nosaukumu datnei galamērķa mapē:
ErrorRegisterServer=Neiespējami reģistrēt DLL/OCX: %1
ErrorRegSvr32Failed=Kļūda, palaižot RegSvr32, kods %1
ErrorRegisterTypeLib=Neiespējami reģistrēt tipa bibliotēku: %1

; *** Uninstall display name markings
UninstallDisplayNameMark=%1 (%2)
UninstallDisplayNameMarks=%1 (%2, %3)
UninstallDisplayNameMark32Bit=32 biti
UninstallDisplayNameMark64Bit=64 biti
UninstallDisplayNameMarkAllUsers=Visi lietotāji
UninstallDisplayNameMarkCurrentUser=Tekošais lietotājs

; *** Post-installation errors
ErrorOpeningReadme=Radusies kļūda, atverot README datni.
ErrorRestartingComputer=Uzstādīšana nevar pārstartēt datoru. Lūdzu, izdariet to manuāli.

; *** Uninstaller messages
UninstallNotFound=Datne "%1" nepastāv. Nevar noņemt.
UninstallOpenError=Datni "%1" nevar atvērt. Nevar noņemt
UninstallUnsupportedVer=Noņemšanas datne "%1" nav atpazīstama šai noņemšanas programmai. Nevar noņemt
UninstallUnknownEntry=Nezināms ieraksts (%1) izveidoja sadursmi ar noņemšanu
ConfirmUninstall=Vai esat pārliecināts, ka vēlaties pilnībā noņemt %1 un visus tā komponentus?
UninstallOnlyOnWin64=Noņemšanu var veikt tikai ar 64-bitu Windows.
OnlyAdminCanUninstall=Noņemšanu var veikt tikai lietotājs ar Adminstratora privilēģijām.
UninstallStatusLabel=Lūdzu uzgaidiet, kamēr %1 tiek noņemts no Jūsu datora.
UninstalledAll=%1 tika veiksmīgi noņemts no Jūsu datora.
UninstalledMost=%1 noņemšana pabeigta.%n%nDažus elementus nevarēja noņemt. Tos var noņemt manuāli.
UninstalledAndNeedsRestart=Lai pabeigtu %1 noņemšanu, Jūsu dators jāpārstartē.%n%nVai vēlaties to darīt tagad?
UninstallDataCorrupted="%1" datne ir bojāta. Nevar noņemt

; *** Uninstallation phase messages
ConfirmDeleteSharedFileTitle=Noņemt kopīgo datni?
ConfirmDeleteSharedFile2=Sistēma ir secinājusi, ka šī koplietošanas datne vairs netiks lietota. Vai vēlaties to noņemt?%n%nJa kāda cita programma izmanto šo datni, tad šī programma var strādāt nekorekti. Ja neesat drošs, izvēlieties "Nē". Atstājot šo datni, Jūsu datoram netiks nodarīti nekādi bojājumi.
SharedFileNameLabel=Faila nosaukums:
SharedFileLocationLabel=Atrašanās vieta:
WizardUninstalling=Noņemšanas statuss
StatusUninstalling=Noņem %1...


; *** Shutdown block reasons
ShutdownBlockReasonInstallingApp=%1 uzstādīšana.
ShutdownBlockReasonUninstallingApp=%1 noņemšana.

; The custom messages below aren't used by Setup itself, but if you make
; use of them in your scripts, you'll want to translate them.

[CustomMessages]

NameAndVersion=%1, versija %2
AdditionalIcons=Papildu ikonas:
CreateDesktopIcon=Izveidot &darbvisrmas ikonu
CreateQuickLaunchIcon=Izveidot &Quick Launch ikonu
ProgramOnTheWeb=%1 vietne Internetā
UninstallProgram=Noņemt %1
LaunchProgram=Palaist %1
AssocFileExtension=&Apvienot %1 ar %2 faila paplašinājumu
AssocingFileExtension=Apvieno %1 ar %2 faila paplašinājumu...
AutoStartProgramGroupDescription=Automātiskā palaišana:
AutoStartProgram=Automātiski palaist %1
AddonHostProgramNotFound=%1 nav atrasts Jūsu norādītajā mapē.%n%nTomēr turpināt?
