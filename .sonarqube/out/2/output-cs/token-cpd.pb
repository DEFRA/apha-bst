∑C
PD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\UserService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class 
UserService 
: 
IUserService +
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
private 
readonly 
IMapper  
_mapper! (
;( )
public 
UserService 
( 
IUserRepository *
userRepository+ 9
,9 :
IMapper; B
mapperC I
)I J
{ 	
_userRepository 
= 
userRepository ,
;, -
_mapper 
= 
mapper 
; 
} 	
public 
async 
Task 
< 
string  
>  !
AddUserAsync" .
(. /
UserDto/ 6
dto7 :
): ;
{ 	
var 
user 
= 
_mapper 
. 
Map "
<" #
User# '
>' (
(( )
dto) ,
), -
;- .
await 
_userRepository !
.! "
AddUserAsync" .
(. /
user/ 3
)3 4
;4 5
var   
	locations   
=   
await   !
GetLocationsAsync  " 3
(  3 4
)  4 5
;  5 6
var!! 
userLocation!! 
=!! 
	locations!! (
.!!( )
FirstOrDefault!!) 7
(!!7 8
l!!8 9
=>!!: <
l!!= >
.!!> ?
LocId!!? D
==!!E G
dto!!H K
.!!K L
UserLoc!!L S
)!!S T
?!!T U
.!!U V
VlaLocation!!V a
??!!b d
$str!!e n
;!!n o
return## 
$"## 
{## 
dto## 
.## 
UserName## "
}##" #
$str### %
{##% &
dto##& )
.##) *
UserId##* 0
}##0 1
$str##1 7
{##7 8
userLocation##8 D
}##D E
$str##E `
{##` a
dto##a d
.##d e
	UserLevel##e n
}##n o
$str##o t
"##t u
;##u v
}$$ 	
public&& 
async&& 
Task&& 
<&& 
List&& 
<&& 
	VlaLocDto&& (
>&&( )
>&&) *
GetLocationsAsync&&+ <
(&&< =
)&&= >
{'' 	
var(( 
locationViews(( 
=(( 
await((  %
_userRepository((& 5
.((5 6
GetLocationsAsync((6 G
(((G H
)((H I
;((I J
return)) 
_mapper)) 
.)) 
Map)) 
<)) 
List)) #
<))# $
	VlaLocDto))$ -
>))- .
>)). /
())/ 0
locationViews))0 =
)))= >
;))> ?
}** 	
public,, 
async,, 
Task,, 
<,, 
List,, 
<,, 
UserViewDto,, *
>,,* +
>,,+ ,
GetUsersAsync,,- :
(,,: ;
string,,; A
userId,,B H
),,H I
{-- 	
var.. 
users.. 
=.. 
await.. 
_userRepository.. -
...- .
GetUsersAsync... ;
(..; <
userId..< B
)..B C
;..C D
return// 
_mapper// 
.// 
Map// 
<// 
List// #
<//# $
UserViewDto//$ /
>/// 0
>//0 1
(//1 2
users//2 7
)//7 8
;//8 9
}00 	
public11 
async11 
Task11 
<11 
string11  
>11  !
UpdateUserAsync11" 1
(111 2
UserDto112 9
dto11: =
)11= >
{22 	
var33 
user33 
=33 
_mapper33 
.33 
Map33 "
<33" #
User33# '
>33' (
(33( )
dto33) ,
)33, -
;33- .
await44 
_userRepository44 !
.44! "
UpdateUserAsync44" 1
(441 2
user442 6
)446 7
;447 8
var77 
users77 
=77 
await77 
_userRepository77 -
.77- .
GetUsersAsync77. ;
(77; <
dto77< ?
.77? @
UserId77@ F
)77F G
;77G H
var88 
updatedUser88 
=88 
users88 #
.88# $
FirstOrDefault88$ 2
(882 3
)883 4
;884 5
var99 
userLocText99 
=99 
updatedUser99 )
?99) *
.99* +
VlaLocation99+ 6
??997 9
$str99: C
;99C D
var:: 
userLevelText:: 
=:: 
updatedUser::  +
?::+ ,
.::, -
UserLevelName::- :
??::; =
$str::> G
;::G H
var== 
message== 
=== 
$"== 
{== 
dto==  
.==  !
UserId==! '
}==' (
$str==( ?
{==? @
dto==@ C
.==C D
UserName==D L
}==L M
$str==M O
{==O P
userLocText==P [
}==[ \
$str==\ ^
{==^ _
userLevelText==_ l
}==l m
"==m n
;==n o
return>> 
message>> 
;>> 
}?? 	
publicAA 
asyncAA 
TaskAA 
<AA 
UserDtoAA !
?AA! "
>AA" #
GetUserByIdAsyncAA$ 4
(AA4 5
stringAA5 ;
userIdAA< B
)AAB C
{BB 	
varCC 
usersCC 
=CC 
awaitCC 
_userRepositoryCC -
.CC- .
GetUsersAsyncCC. ;
(CC; <
userIdCC< B
)CCB C
;CCC D
varDD 
userViewDD 
=DD 
usersDD  
.DD  !
FirstOrDefaultDD! /
(DD/ 0
)DD0 1
;DD1 2
ifFF 
(FF 
userViewFF 
==FF 
nullFF  
)FF  !
returnGG 
nullGG 
;GG 
varJJ 
userDtoJJ 
=JJ 
newJJ 
UserDtoJJ %
{KK 
UserIdLL 
=LL 
userViewLL !
.LL! "
UserIdLL" (
,LL( )
UserNameMM 
=MM 
userViewMM #
.MM# $
UserNameMM$ ,
,MM, -
UserLocNN 
=NN 
userViewNN "
.NN" #
UserLocNN# *
,NN* +
	UserLevelOO 
=OO 
userViewOO $
.OO$ %
	UserLevelOO% .
}PP 
;PP 
returnRR 
userDtoRR 
;RR 
}SS 	
publicUU 
asyncUU 
TaskUU 
<UU 
stringUU  
>UU  !
DeleteUserAsyncUU" 1
(UU1 2
stringUU2 8
userIdUU9 ?
)UU? @
{VV 	
varWW 
usersWW 
=WW 
awaitWW 
_userRepositoryWW -
.WW- .
GetUsersAsyncWW. ;
(WW; <
userIdWW< B
)WWB C
;WWC D
varXX 
userXX 
=XX 
usersXX 
.XX 
FirstOrDefaultXX +
(XX+ ,
)XX, -
;XX- .
ifYY 
(YY 
userYY 
==YY 
nullYY 
)YY 
returnZZ 
$"ZZ 
$strZZ &
{ZZ& '
userIdZZ' -
}ZZ- .
$strZZ. 9
"ZZ9 :
;ZZ: ;
await\\ 
_userRepository\\ !
.\\! "
DeleteUserAsync\\" 1
(\\1 2
userId\\2 8
)\\8 9
;\\9 :
var^^ 
message^^ 
=^^ 
$"^^ 
{^^ 
user^^ !
.^^! "
UserId^^" (
}^^( )
$str^^) ,
{^^, -
user^^- 1
.^^1 2
UserName^^2 :
}^^: ;
$str^^; ^
"^^^ _
;^^_ `
return__ 
message__ 
;__ 
}`` 	
}bb 
}cc –j
TD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\TrainingService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class 
TrainingService  
:  !
ITrainingService" 2
{ 
private 
readonly 
ITrainingRepository ,
_trainingRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
public 
const 
string 
Fail  
=! "
$str# )
;) *
public 
const 
string 
Exists "
=# $
$str% -
;- .
public 
TrainingService 
( 
ITrainingRepository 2
trainingRepository3 E
,E F
IMapperG N
mapperO U
)U V
{ 	
_trainingRepository 
=  !
trainingRepository" 4
;4 5
_mapper 
= 
mapper 
; 
} 	
public 
async 
Task 
< 
List 
< 

TraineeDto )
>) *
>* +
GetTraineesAsync, <
(< =
)= >
{ 	
var 
persons 
= 
await 
_trainingRepository  3
.3 4
GetAllTraineesAsync4 G
(G H
)H I
;I J
return 
_mapper 
. 
Map 
< 
List #
<# $

TraineeDto$ .
>. /
>/ 0
(0 1
persons1 8
)8 9
;9 :
} 	
public   
async   
Task   
<   
IEnumerable   %
<  % &
TrainerTrainingDto  & 8
>  8 9
>  9 :%
GetTrainingByTraineeAsync  ; T
(  T U
string  U [
	traineeId  \ e
)  e f
{!! 	
var"" 
	trainings"" 
="" 
await"" !
_trainingRepository""" 5
.""5 6%
GetTrainingByTraineeAsync""6 O
(""O P
	traineeId""P Y
)""Y Z
;""Z [
return## 
_mapper## 
.## 
Map## 
<## 
IEnumerable## *
<##* +
TrainerTrainingDto##+ =
>##= >
>##> ?
(##? @
	trainings##@ I
)##I J
;##J K
}$$ 	
public%% 
async%% 
Task%% 
<%% 
IEnumerable%% %
<%%% &
TrainerTrainingDto%%& 8
>%%8 9
>%%9 : 
GetAllTrainingsAsync%%; O
(%%O P
)%%P Q
{&& 	
var'' 
	trainings'' 
='' 
await'' !
_trainingRepository''" 5
.''5 6 
GetAllTrainingsAsync''6 J
(''J K
)''K L
;''L M
return(( 
_mapper(( 
.(( 
Map(( 
<(( 
IEnumerable(( *
<((* +
TrainerTrainingDto((+ =
>((= >
>((> ?
(((? @
	trainings((@ I
)((I J
;((J K
})) 	
public,, 
async,, 
Task,, 
<,, 
TrainingDto,, %
?,,% &
>,,& '"
GetTrainingByKeysAsync,,( >
(,,> ?
int,,? B
	traineeId,,C L
,,,L M
int,,N Q
	trainerId,,R [
,,,[ \
string,,] c
species,,d k
,,,k l
DateTime,,m u
dateTrained	,,v Å
,
,,Å Ç
string
,,É â
trainingType
,,ä ñ
)
,,ñ ó
{-- 	
var.. 
training.. 
=.. 
await..  
_trainingRepository..! 4
...4 5"
GetTrainingByKeysAsync..5 K
(..K L
	traineeId..L U
,..U V
	trainerId..W `
,..` a
species..b i
,..i j
dateTrained..k v
,..v w
trainingType	..x Ñ
)
..Ñ Ö
;
..Ö Ü
return// 
_mapper// 
.// 
Map// 
<// 
TrainingDto// *
>//* +
(//+ ,
training//, 4
)//4 5
;//5 6
}00 	
public22 
async22 
Task22 
<22 
string22  
>22  !
UpdateTrainingAsync22" 5
(225 6
EditTrainingDto226 E
dto22F I
)22I J
{33 	
if44 
(44 
dto44 
.44 
TraineeIdOld44  
==44! #
dto44$ '
.44' (
	TrainerId44( 1
)441 2
{55 
return66 
$str66 G
;66G H
}77 
var88 
editTraining88 
=88 
_mapper88 &
.88& '
Map88' *
<88* +
EditTraining88+ 7
>887 8
(888 9
dto889 <
)88< =
;88= >
var;; 
trainee;; 
=;; 
await;; 
_trainingRepository;;  3
.;;3 4
GetPersonByIdAsync;;4 F
(;;F G
dto;;G J
.;;J K
	TraineeId;;K T
);;T U
;;;U V
var<< 
trainer<< 
=<< 
await<< 
_trainingRepository<<  3
.<<3 4
GetPersonByIdAsync<<4 F
(<<F G
dto<<G J
.<<J K
	TrainerId<<K T
)<<T U
;<<U V
string>> 
traineeName>> 
=>>  
trainee>>! (
?>>( )
.>>) *
Person>>* 0
??>>1 3
dto>>4 7
.>>7 8
	TraineeId>>8 A
.>>A B
ToString>>B J
(>>J K
)>>K L
;>>L M
string?? 
trainerName?? 
=??  
trainer??! (
???( )
.??) *
Person??* 0
????1 3
dto??4 7
.??7 8
	TrainerId??8 A
.??A B
ToString??B J
(??J K
)??K L
;??L M
var@@ 
result@@ 
=@@ 
await@@ 
_trainingRepository@@ 2
.@@2 3
UpdateTrainingAsync@@3 F
(@@F G
editTraining@@G S
)@@S T
;@@T U
ifAA 
(AA 
resultAA 
.AA 

StartsWithAA !
(AA! "
FailAA" &
)AA& '
)AA' (
{BB 
returnCC 
$"CC 
$strCC %
"CC% &
;CC& '
}DD 
returnFF 
$"FF 
{FF 
traineeNameFF !
}FF! "
$strFF" 7
{FF7 8
dtoFF8 ;
.FF; <
TrainingAnimalFF< J
}FFJ K
$strFFK a
{FFa b
dtoFFb e
.FFe f
TrainingDateTimeFFf v
:FFv w
$str	FFw Å
}
FFÅ Ç
$str
FFÇ Ü
{
FFÜ á
trainerName
FFá í
}
FFí ì
"
FFì î
;
FFî ï
}HH 	
publicKK 
asyncKK 
TaskKK 
<KK 
IEnumerableKK %
<KK% &
TrainerHistoryDtoKK& 7
>KK7 8
>KK8 9"
GetTrainerHistoryAsyncKK: P
(KKP Q
intKKQ T
personIdKKU ]
,KK] ^
stringKK_ e

animalTypeKKf p
)KKp q
{LL 	
varMM 
historyMM 
=MM 
awaitMM 
_trainingRepositoryMM  3
.MM3 4"
GetTrainerHistoryAsyncMM4 J
(MMJ K
personIdMMK S
,MMS T

animalTypeMMU _
)MM_ `
;MM` a
returnNN 
_mapperNN 
.NN 
MapNN 
<NN 
IEnumerableNN *
<NN* +
TrainerHistoryDtoNN+ <
>NN< =
>NN= >
(NN> ?
historyNN? F
)NNF G
;NNG H
}OO 	
publicRR 
asyncRR 
TaskRR 
<RR 
IEnumerableRR %
<RR% &
TrainerTrainedDtoRR& 7
>RR7 8
>RR8 9"
GetTrainerTrainedAsyncRR: P
(RRP Q
intRRQ T
	trainerIdRRU ^
)RR^ _
{SS 	
varTT 
trainedTT 
=TT 
awaitTT 
_trainingRepositoryTT  3
.TT3 4"
GetTrainerTrainedAsyncTT4 J
(TTJ K
	trainerIdTTK T
)TTT U
;TTU V
returnUU 
_mapperUU 
.UU 
MapUU 
<UU 
IEnumerableUU *
<UU* +
TrainerTrainedDtoUU+ <
>UU< =
>UU= >
(UU> ?
trainedUU? F
)UUF G
;UUG H
}VV 	
publicXX 
asyncXX 
TaskXX 
<XX 
stringXX  
>XX  !
AddTrainingAsyncXX" 2
(XX2 3
TrainingDtoXX3 >
trainingDtoXX? J
)XXJ K
{YY 	
ifZZ 
(ZZ 
trainingDtoZZ 
.ZZ 
PersonIdZZ $
==ZZ% '
trainingDtoZZ( 3
.ZZ3 4
	TrainerIdZZ4 =
)ZZ= >
{[[ 
return\\ 
$str\\ G
;\\G H
}]] 
var^^ 
training^^ 
=^^ 
_mapper^^ "
.^^" #
Map^^# &
<^^& '
Training^^' /
>^^/ 0
(^^0 1
trainingDto^^1 <
)^^< =
;^^= >
var__ 
result__ 
=__ 
await__ 
_trainingRepository__ 2
.__2 3
AddTrainingAsync__3 C
(__C D
training__D L
)__L M
;__M N
varaa 
traineeaa 
=aa 
awaitaa 
_trainingRepositoryaa  3
.aa3 4
GetPersonByIdAsyncaa4 F
(aaF G
trainingDtoaaG R
.aaR S
PersonIdaaS [
)aa[ \
;aa\ ]
varbb 
trainerbb 
=bb 
awaitbb 
_trainingRepositorybb  3
.bb3 4
GetPersonByIdAsyncbb4 F
(bbF G
trainingDtobbG R
.bbR S
	TrainerIdbbS \
)bb\ ]
;bb] ^
stringdd 
traineeNamedd 
=dd  
traineedd! (
?dd( )
.dd) *
Persondd* 0
??dd1 3
trainingDtodd4 ?
.dd? @
PersonIddd@ H
.ddH I
ToStringddI Q
(ddQ R
)ddR S
;ddS T
stringee 
trainerNameee 
=ee  
traineree! (
?ee( )
.ee) *
Personee* 0
??ee1 3
trainingDtoee4 ?
.ee? @
	TrainerIdee@ I
.eeI J
ToStringeeJ R
(eeR S
)eeS T
;eeT U
ifgg 
(gg 
resultgg 
==gg 
Existsgg  
)gg  !
{hh 
returnii 
$"ii 
{ii 
traineeNameii %
}ii% &
$strii& ?
{ii? @
trainingDtoii@ K
.iiK L
TrainingTypeiiL X
}iiX Y
$striiY 
"	ii Ä
;
iiÄ Å
}jj 
returnll 
$"ll 
{ll 
traineeNamell !
}ll! "
$strll" 7
{ll7 8
trainingDtoll8 C
.llC D
TrainingTypellD P
}llP Q
$strllQ g
{llg h
trainingDtollh s
.lls t
TrainingDateTime	llt Ñ
:
llÑ Ö
$str
llÖ è
}
llè ê
$str
llê î
{
llî ï
trainerName
llï †
}
ll† °
"
ll° ¢
;
ll¢ £
}nn 	
publicoo 
asyncoo 
Taskoo 
<oo 
stringoo  
>oo  !
DeleteTrainingAsyncoo" 5
(oo5 6
intoo6 9
	traineeIdoo: C
,ooC D
stringooE K
speciesooL S
,ooS T
DateTimeooU ]
dateTrainedoo^ i
)ooi j
{pp 	
varqq 
resultqq 
=qq 
awaitqq 
_trainingRepositoryqq 2
.qq2 3
DeleteTrainingAsyncqq3 F
(qqF G
	traineeIdqqG P
,qqP Q
speciesqqR Y
,qqY Z
dateTrainedqq[ f
)qqf g
;qqg h
varrr 
traineerr 
=rr 
awaitrr 
_trainingRepositoryrr  3
.rr3 4
GetPersonByIdAsyncrr4 F
(rrF G
	traineeIdrrG P
)rrP Q
;rrQ R
stringss 
traineeNamess 
=ss  
traineess! (
?ss( )
.ss) *
Personss* 0
??ss1 3
	traineeIdss4 =
.ss= >
ToStringss> F
(ssF G
)ssG H
;ssH I
ifuu 
(uu 
resultuu 
.uu 

StartsWithuu !
(uu! "
Failuu" &
)uu& '
)uu' (
{vv 
returnww 
$"ww 
$strww '
"ww' (
;ww( )
}xx 
returnzz 
$"zz 
{zz 
traineeNamezz !
}zz! "
$strzz" .
{zz. /
specieszz/ 6
}zz6 7
$strzz7 M
{zzM N
dateTrainedzzN Y
:zzY Z
$strzzZ d
}zzd e
$str	zze à
"
zzà â
;
zzâ ä
}{{ 	
}|| 
}}} Ñ
ZD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\StaticDropdownService.cs
	namespace		 	
Apha		
 
.		 
BST		 
.		 
Application		 
.		 
Services		 '
{

 
public 

class !
StaticDropdownService &
:' ("
IStaticDropdownService) ?
{ 
public 
List 
< 
SelectListItem "
>" #
GetTrainingTypes$ 4
(4 5
)5 6
=>7 9
new: =
List> B
<B C
SelectListItemC Q
>Q R
{ 
new 
SelectListItem 
{  
Value! &
=' (
$str) :
,: ;
Text< @
=A B
$strC T
}U V
,V W
new 
SelectListItem 
{  
Value! &
=' (
$str) 2
,2 3
Text4 8
=9 :
$str; D
}E F
,F G
new 
SelectListItem 
{  
Value! &
=' (
$str) =
,= >
Text? C
=D E
$strF Z
}[ \
} 
; 	
public 
List 
< 
SelectListItem "
>" #
GetTrainingAnimal$ 5
(5 6
)6 7
=>8 :
new; >
List? C
<C D
SelectListItemD R
>R S
{ 	
new 
SelectListItem 
{  
Value! &
=' (
$str) 1
,1 2
Text3 7
=8 9
$str: B
}C D
,D E
new 
SelectListItem 
{  
Value! &
=' (
$str) 9
,9 :
Text; ?
=@ A
$strB R
}S T
} 	
;	 

} 
} •1
PD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\SiteService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class 
SiteService 
: 
ISiteService *
{ 
private 
readonly 
ISiteRepository (
_siteRepository) 8
;8 9
private 
readonly 
IMapper  
_mapper! (
;( )
public 
const 
string 
Exists "
=# $
$str% -
;- .
public 
SiteService 
( 
ISiteRepository *
siteRepository+ 9
,9 :
IMapper; B
mapperC I
)I J
{ 	
_siteRepository 
= 
siteRepository ,
??- /
throw0 5
new6 9!
ArgumentNullException: O
(O P
nameofP V
(V W
siteRepositoryW e
)e f
)f g
;g h
_mapper 
= 
mapper 
?? 
throw  %
new& )!
ArgumentNullException* ?
(? @
nameof@ F
(F G
mapperG M
)M N
)N O
;O P
} 	
public 
async 
Task 
< 
IEnumerable %
<% &
SiteDto& -
>- .
>. /
GetAllSitesAsync0 @
(@ A
stringA G
plantNoH O
)O P
{ 	
var   
sites   
=   
await   !
_siteRepository  " 1
.  1 2
GetAllSitesAsync  2 B
(  B C
plantNo  C J
)  J K
;  K L
return!! 
_mapper!! 
.!! 
Map!! "
<!!" #
IEnumerable!!# .
<!!. /
SiteDto!!/ 6
>!!6 7
>!!7 8
(!!8 9
sites!!9 >
)!!> ?
;!!? @
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
SiteTraineeDto$$ -
>$$- .
>$$. / 
GetSiteTraineesAsync$$0 D
($$D E
string$$E K
plantNo$$L S
)$$S T
{%% 	
var&& 
trainees&& 
=&& 
await&&  
_siteRepository&&! 0
.&&0 1 
GetSiteTraineesAsync&&1 E
(&&E F
plantNo&&F M
)&&M N
;&&N O
return'' 
trainees'' 
.'' 
Select'' "
(''" #
t''# $
=>''% '
new''( +
SiteTraineeDto'', :
{(( 
PersonId)) 
=)) 
t)) 
.)) 
PersonId)) %
,))% &
Person** 
=** 
t** 
.** 
Person** !
}++ 
)++ 
.++ 
ToList++ 
(++ 
)++ 
;++ 
},, 	
public// 
async// 
Task// 
<// 
string//  
>//  !
AddSiteAsync//" .
(//. /
SiteDto/// 6
siteDto//7 >
,//> ?
string//@ F
userName//G O
)//O P
{00 	
var11 
site11 
=11 
_mapper11 
.11 
Map11 "
<11" #
Site11# '
>11' (
(11( )
siteDto11) 0
)110 1
;111 2
var22 
createdSite22 
=22 
await22 #
_siteRepository22$ 3
.223 4
AddSiteAsync224 @
(22@ A
site22A E
,22E F
userName22F N
)22N O
;22O P
if33 
(33 
createdSite33 
==33 
Exists33 %
)33% &
{44 
return55 
$str55 T
;55T U
}66 
return88 
$"88 
$str88 
{88 
siteDto88 
.88 
Name88 #
}88# $
$str88$ 3
"883 4
;884 5
}:: 	
public;; 
async;; 
Task;; 
<;; 
string;;  
>;;  !
DeleteTraineeAsync;;" 4
(;;4 5
int;;5 8
personId;;9 A
);;A B
{<< 	
var>> 

personName>> 
=>> 
await>> "
_siteRepository>># 2
.>>2 3"
GetPersonNameByIdAsync>>3 I
(>>I J
personId>>J R
)>>R S
;>>S T
var@@ 
deleted@@ 
=@@ 
await@@ 
_siteRepository@@  /
.@@/ 0
DeleteTraineeAsync@@0 B
(@@B C
personId@@C K
)@@K L
;@@L M
ifBB 
(BB 
deletedBB 
)BB 
{CC 
returnDD 
$"DD 
$strDD "
{DD" #

personNameDD# -
}DD- .
$strDD. E
"DDE F
;DDF G
}EE 
elseFF 
{GG 
returnHH 
$"HH 
$strHH "
{HH" #

personNameHH# -
}HH- .
$strHH. y
"HHy z
;HHz {
}II 
}JJ 	
publicKK 
asyncKK 
TaskKK 
<KK 
stringKK  
>KK  !
UpdateSiteAsyncKK" 1
(KK1 2
SiteInputDtoKK2 >
siteInputDtoKK? K
)KKK L
{LL 	
varMM 
	siteInputMM 
=MM 
_mapperMM #
.MM# $
MapMM$ '
<MM' (
	SiteInputMM( 1
>MM1 2
(MM2 3
siteInputDtoMM3 ?
)MM? @
;MM@ A
awaitNN 
_siteRepositoryNN !
.NN! "
UpdateSiteAsyncNN" 1
(NN1 2
	siteInputNN2 ;
)NN; <
;NN< =
returnOO 
$"OO 
{OO 
	siteInputOO 
.OO  
NameOO  $
}OO$ %
$strOO% ;
"OO; <
;OO< =
}QQ 	
}RR 
}SS Ü
WD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\RoleMappingService.cs
	namespace		 	
Apha		
 
.		 
BST		 
.		 
Application		 
.		 
Services		 '
{

 
public 

class 
RoleMappingService #
:$ %
IRoleMappingService& 9
{ 
private 
static 
readonly 

Dictionary  *
<* +
byte+ /
,/ 0
string1 7
>7 8
_roleMappings9 F
=G H
newI L

DictionaryM W
<W X
byteX \
,\ ]
string^ d
>d e
{ 	
{ 
$num 
, 
$str 
} 
, 
{ 
$num 
, 
$str 
} 
, 
{ 
$num 
, 
$str 
} 
} 	
;	 

public 
async 
Task 
< 
string  
>  !
GetRoleName# .
(. /
byte/ 3
roleId4 :
): ;
{ 	
var 
roleName 
= 
_roleMappings (
.( )
TryGetValue) 4
(4 5
roleId5 ;
,; <
out= @
varA D
nameE I
)I J
?K L
nameM Q
:R S
$strT b
;b c
return 
await 
Task 
. 

FromResult (
(( )
roleName) 1
)1 2
;2 3
} 	
public 
List 
< 
SelectListItem "
>" #
GetUserLevels$ 1
(1 2
)2 3
=>4 6
new7 :
List; ?
<? @
SelectListItem@ N
>N O
{ 	
new 
SelectListItem 
{  
Value! &
=' (
$str) ,
,, -
Text. 2
=3 4
$str5 J
}K L
,L M
new 
SelectListItem 
{  
Value! &
=' (
$str) ,
,, -
Text. 2
=3 4
$str5 K
}L M
,M N
new 
SelectListItem 
{  
Value! &
=' (
$str) ,
,, -
Text. 2
=3 4
$str5 J
}K L
} 	
;	 

}   
}!! «Ç
RD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\ReportService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class 
ReportService 
:  
IReportService! /
{ 
private 
readonly 
IReportRepository *
_reportRepository+ <
;< =
private 
readonly 
IMapper  
_mapper! (
;( )
private 
readonly 
IConfiguration '
_configuration( 6
;6 7
public 
ReportService 
( 
IReportRepository .
reportRepository/ ?
,? @
IMapper #
mapper$ *
,* +
IConfiguration *
configuration+ 8
)8 9
{ 	
_reportRepository 
= 
reportRepository  0
;0 1
_mapper 
= 
mapper 
; 
_configuration 
= 
configuration *
;* +
} 	
public!! 
async!! 
Task!! 
<!! 
List!! 
<!! 
AphaReportDto!! ,
>!!, -
>!!- .
GetAphaReportsAsync!!/ B
(!!B C
)!!C D
{"" 	
var## 
aphaReports## 
=## 
await## #
_reportRepository##$ 5
.##5 6
GetAphaReportsAsync##6 I
(##I J
)##J K
;##K L
return$$ 
_mapper$$ 
.$$ 
Map$$ 
<$$ 
List$$ #
<$$# $
AphaReportDto$$$ 1
>$$1 2
>$$2 3
($$3 4
aphaReports$$4 ?
)$$? @
;$$@ A
}%% 	
public'' 
async'' 
Task'' 
<'' 
List'' 
<'' 
PeopleReportDto'' .
>''. /
>''/ 0!
GetPeopleReportsAsync''1 F
(''F G
)''G H
{(( 	
var)) 
peopleReports)) 
=)) 
await))  %
_reportRepository))& 7
.))7 8!
GetPeopleReportsAsync))8 M
())M N
)))N O
;))O P
return** 
_mapper** 
.** 
Map** 
<** 
List** #
<**# $
PeopleReportDto**$ 3
>**3 4
>**4 5
(**5 6
peopleReports**6 C
)**C D
;**D E
}++ 	
public-- 
async-- 
Task-- 
<-- 
List-- 
<-- 
SiteReportDto-- ,
>--, -
>--- .
GetSiteReportsAsync--/ B
(--B C
)--C D
{.. 	
var// 
siteReports// 
=// 
await// #
_reportRepository//$ 5
.//5 6
GetSiteReportsAsync//6 I
(//I J
)//J K
;//K L
return00 
_mapper00 
.00 
Map00 
<00 
List00 #
<00# $
SiteReportDto00$ 1
>001 2
>002 3
(003 4
siteReports004 ?
)00? @
;00@ A
}11 	
public33 
async33 
Task33 
<33 
List33 
<33 
TrainerReportDto33 /
>33/ 0
>330 1"
GetTrainerReportsAsync332 H
(33H I
)33I J
{44 	
var55 
trainerReports55 
=55  
await55! &
_reportRepository55' 8
.558 9"
GetTrainerReportsAsync559 O
(55O P
)55P Q
;55Q R
return66 
_mapper66 
.66 
Map66 
<66 
List66 #
<66# $
TrainerReportDto66$ 4
>664 5
>665 6
(666 7
trainerReports667 E
)66E F
;66F G
}77 	
public99 
async99 
Task99 
<99 
List99 
<99 
TrainingReportDto99 0
>990 1
>991 2#
GetTrainingReportsAsync993 J
(99J K
)99K L
{:: 	
var;; 
trainingReports;; 
=;;  !
await;;" '
_reportRepository;;( 9
.;;9 :#
GetTrainingReportsAsync;;: Q
(;;Q R
);;R S
;;;S T
return<< 
_mapper<< 
.<< 
Map<< 
<<< 
List<< #
<<<# $
TrainingReportDto<<$ 5
><<5 6
><<6 7
(<<7 8
trainingReports<<8 G
)<<G H
;<<H I
}== 	
public?? 
async?? 
Task?? 
<?? 
(?? 
byte?? 
[??  
]??  !
FileContents??" .
,??. /
string??0 6
FileName??7 ?
)??? @
>??@ A$
GenerateExcelReportAsync??B Z
(??Z [
)??[ \
{@@ 	
stringAA 
dateNowAA 
=AA 
$"AA 
$strAA '
{AA' (
DateTimeAA( 0
.AA0 1
TodayAA1 6
:AA6 7
$strAA7 A
}AAA B
$strAAB G
"AAG H
;AAH I
stringBB 
?BB 
templatePathBB  
=BB! "
_configurationBB# 1
[BB1 2
$strBB2 S
]BBS T
;BBT U
ifDD 
(DD 
stringDD 
.DD 
IsNullOrWhiteSpaceDD )
(DD) *
templatePathDD* 6
)DD6 7
)DD7 8
throwEE 
newEE %
InvalidOperationExceptionEE 3
(EE3 4
$strEE4 b
)EEb c
;EEc d
varHH 
siteReportsHH 
=HH 
awaitHH #
GetSiteReportsAsyncHH$ 7
(HH7 8
)HH8 9
;HH9 :
varII 
trainerReportsII 
=II  
awaitII! &"
GetTrainerReportsAsyncII' =
(II= >
)II> ?
;II? @
varJJ 
peopleReportsJJ 
=JJ 
awaitJJ  %!
GetPeopleReportsAsyncJJ& ;
(JJ; <
)JJ< =
;JJ= >
varKK 
trainingReportsKK 
=KK  !
awaitKK" '#
GetTrainingReportsAsyncKK( ?
(KK? @
)KK@ A
;KKA B
varLL 
aphaReportsLL 
=LL 
awaitLL #
GetAphaReportsAsyncLL$ 7
(LL7 8
)LL8 9
;LL9 :
usingNN 
varNN 
memoryStreamNN "
=NN# $
newNN% (
MemoryStreamNN) 5
(NN5 6
)NN6 7
;NN7 8
usingOO 
(OO 
varOO 
templateStreamOO %
=OO& '
FileOO( ,
.OO, -
OpenReadOO- 5
(OO5 6
templatePathOO6 B
)OOB C
)OOC D
usingPP 
(PP 
varPP 
workbookPP 
=PP  !
newPP" %

XLWorkbookPP& 0
(PP0 1
templateStreamPP1 ?
)PP? @
)PP@ A
{QQ 
FillSitesSheetRR 
(RR 
workbookRR '
,RR' (
siteReportsRR) 4
)RR4 5
;RR5 6
FillPeopleSheetSS 
(SS  
workbookSS  (
,SS( )
peopleReportsSS* 7
)SS7 8
;SS8 9
FillTrainersSheetTT !
(TT! "
workbookTT" *
,TT* +
trainerReportsTT, :
)TT: ;
;TT; <
FillTrainingSheetUU !
(UU! "
workbookUU" *
,UU* +
trainingReportsUU, ;
)UU; <
;UU< =
FillLocationsSheetVV "
(VV" #
workbookVV# +
,VV+ ,
aphaReportsVV- 8
)VV8 9
;VV9 :
workbookXX 
.XX 
SaveAsXX 
(XX  
memoryStreamXX  ,
)XX, -
;XX- .
}YY 
return[[ 
([[ 
memoryStream[[  
.[[  !
ToArray[[! (
([[( )
)[[) *
,[[* +
dateNow[[, 3
)[[3 4
;[[4 5
}\\ 	
private`` 
static`` 
void`` 
FillSitesSheet`` *
(``* +

XLWorkbook``+ 5
workbook``6 >
,``> ?
List``@ D
<``D E
SiteReportDto``E R
>``R S
sites``T Y
)``Y Z
{aa 	
varbb 
wsbb 
=bb 
workbookbb 
.bb 

Worksheetsbb (
.bb( )
FirstOrDefaultbb) 7
(bb7 8
wbb8 9
=>bb: <
wbb= >
.bb> ?
Namebb? C
==bbD F
$strbbG N
)bbN O
??bbP R
workbookbbS [
.bb[ \

Worksheetsbb\ f
.bbf g
Addbbg j
(bbj k
$strbbk r
)bbr s
;bbs t
wscc 
.cc 
Clearcc 
(cc 
)cc 
;cc 
wsdd 
.dd 
Celldd 
(dd 
$numdd 
,dd 
$numdd 
)dd 
.dd 
Valuedd 
=dd  !
$strdd" ,
;dd, -
wsee 
.ee 
Cellee 
(ee 
$numee 
,ee 
$numee 
)ee 
.ee 
Valueee 
=ee  !
$stree" (
;ee( )
wsff 
.ff 
Cellff 
(ff 
$numff 
,ff 
$numff 
)ff 
.ff 
Valueff 
=ff  !
$strff" -
;ff- .
wsgg 
.gg 
Cellgg 
(gg 
$numgg 
,gg 
$numgg 
)gg 
.gg 
Valuegg 
=gg  !
$strgg" -
;gg- .
wshh 
.hh 
Cellhh 
(hh 
$numhh 
,hh 
$numhh 
)hh 
.hh 
Valuehh 
=hh  !
$strhh" (
;hh( )
wsii 
.ii 
Cellii 
(ii 
$numii 
,ii 
$numii 
)ii 
.ii 
Valueii 
=ii  !
$strii" *
;ii* +
wsjj 
.jj 
Celljj 
(jj 
$numjj 
,jj 
$numjj 
)jj 
.jj 
Valuejj 
=jj  !
$strjj" ,
;jj, -
wskk 
.kk 
Cellkk 
(kk 
$numkk 
,kk 
$numkk 
)kk 
.kk 
Valuekk 
=kk  !
$strkk" -
;kk- .
wsll 
.ll 
Cellll 
(ll 
$numll 
,ll 
$numll 
)ll 
.ll 
Valuell 
=ll  !
$strll" '
;ll' (
wsmm 
.mm 
Cellmm 
(mm 
$nummm 
,mm 
$nummm 
)mm 
.mm 
Valuemm  
=mm! "
$strmm# *
;mm* +
wsnn 
.nn 
Cellnn 
(nn 
$numnn 
,nn 
$numnn 
)nn 
.nn 
Valuenn  
=nn! "
$strnn# .
;nn. /
wsoo 
.oo 
Celloo 
(oo 
$numoo 
,oo 
$numoo 
)oo 
.oo 
Valueoo  
=oo! "
$stroo# *
;oo* +
wspp 
.pp 
Rangepp 
(pp 
$numpp 
,pp 
$numpp 
,pp 
$numpp 
,pp 
$numpp  
)pp  !
.pp! "
Stylepp" '
.pp' (
Fontpp( ,
.pp, -
Boldpp- 1
=pp2 3
truepp4 8
;pp8 9
intrr 
rowrr 
=rr 
$numrr 
;rr 
foreachss 
(ss 
varss 
sitess 
inss  
sitesss! &
)ss& '
{tt 
wsuu 
.uu 
Celluu 
(uu 
rowuu 
,uu 
$numuu 
)uu 
.uu  
Valueuu  %
=uu& '
siteuu( ,
.uu, -
PlantNouu- 4
;uu4 5
varvv 
nameCellvv 
=vv 
wsvv !
.vv! "
Cellvv" &
(vv& '
rowvv' *
,vv* +
$numvv, -
)vv- .
;vv. /
nameCellww 
.ww 
Valueww 
=ww  
siteww! %
.ww% &
Nameww& *
;ww* +
nameCellxx 
.xx 
SetHyperlinkxx %
(xx% &
newxx& )
XLHyperlinkxx* 5
(xx5 6
$strxx6 C
)xxC D
)xxD E
;xxE F
wsyy 
.yy 
Cellyy 
(yy 
rowyy 
,yy 
$numyy 
)yy 
.yy  
Valueyy  %
=yy& '
siteyy( ,
.yy, -
Add1yy- 1
;yy1 2
wszz 
.zz 
Cellzz 
(zz 
rowzz 
,zz 
$numzz 
)zz 
.zz  
Valuezz  %
=zz& '
sitezz( ,
.zz, -
Add2zz- 1
;zz1 2
ws{{ 
.{{ 
Cell{{ 
({{ 
row{{ 
,{{ 
$num{{ 
){{ 
.{{  
Value{{  %
={{& '
site{{( ,
.{{, -
Town{{- 1
;{{1 2
ws|| 
.|| 
Cell|| 
(|| 
row|| 
,|| 
$num|| 
)|| 
.||  
Value||  %
=||& '
site||( ,
.||, -
County||- 3
;||3 4
ws}} 
.}} 
Cell}} 
(}} 
row}} 
,}} 
$num}} 
)}} 
.}}  
Value}}  %
=}}& '
site}}( ,
.}}, -
Postcode}}- 5
;}}5 6
ws~~ 
.~~ 
Cell~~ 
(~~ 
row~~ 
,~~ 
$num~~ 
)~~ 
.~~  
Value~~  %
=~~& '
site~~( ,
.~~, -
Phone~~- 2
;~~2 3
ws 
. 
Cell 
( 
row 
, 
$num 
) 
.  
Value  %
=& '
site( ,
., -
Fax- 0
;0 1
ws
ÄÄ 
.
ÄÄ 
Cell
ÄÄ 
(
ÄÄ 
row
ÄÄ 
,
ÄÄ 
$num
ÄÄ 
)
ÄÄ  
.
ÄÄ  !
Value
ÄÄ! &
=
ÄÄ' (
site
ÄÄ) -
.
ÄÄ- .
People
ÄÄ. 4
;
ÄÄ4 5
ws
ÅÅ 
.
ÅÅ 
Cell
ÅÅ 
(
ÅÅ 
row
ÅÅ 
,
ÅÅ 
$num
ÅÅ 
)
ÅÅ  
.
ÅÅ  !
Value
ÅÅ! &
=
ÅÅ' (
site
ÅÅ) -
.
ÅÅ- .
RunTot
ÅÅ. 4
;
ÅÅ4 5
ws
ÇÇ 
.
ÇÇ 
Cell
ÇÇ 
(
ÇÇ 
row
ÇÇ 
,
ÇÇ 
$num
ÇÇ 
)
ÇÇ  
.
ÇÇ  !
Value
ÇÇ! &
=
ÇÇ' (
site
ÇÇ) -
.
ÇÇ- .
Excel
ÇÇ. 3
;
ÇÇ3 4
row
ÉÉ 
++
ÉÉ 
;
ÉÉ 
}
ÑÑ 
}
ÖÖ 	
private
áá 
static
áá 
void
áá 
FillPeopleSheet
áá +
(
áá+ ,

XLWorkbook
áá, 6
workbook
áá7 ?
,
áá? @
List
ááA E
<
ááE F
PeopleReportDto
ááF U
>
ááU V
people
ááW ]
)
áá] ^
{
àà 	
var
ââ 
ws
ââ 
=
ââ 
workbook
ââ 
.
ââ 

Worksheets
ââ (
.
ââ( )
FirstOrDefault
ââ) 7
(
ââ7 8
w
ââ8 9
=>
ââ: <
w
ââ= >
.
ââ> ?
Name
ââ? C
==
ââD F
$str
ââG O
)
ââO P
??
ââQ S
workbook
ââT \
.
ââ\ ]

Worksheets
ââ] g
.
ââg h
Add
ââh k
(
ââk l
$str
ââl t
)
âât u
;
ââu v
ws
ää 
.
ää 
Clear
ää 
(
ää 
)
ää 
;
ää 
ws
ãã 
.
ãã 
Cell
ãã 
(
ãã 
$num
ãã 
,
ãã 
$num
ãã 
)
ãã 
.
ãã 
Value
ãã 
=
ãã  !
$str
ãã" &
;
ãã& '
ws
åå 
.
åå 
Cell
åå 
(
åå 
$num
åå 
,
åå 
$num
åå 
)
åå 
.
åå 
Value
åå 
=
åå  !
$str
åå" *
;
åå* +
ws
çç 
.
çç 
Cell
çç 
(
çç 
$num
çç 
,
çç 
$num
çç 
)
çç 
.
çç 
Value
çç 
=
çç  !
$str
çç" /
;
çç/ 0
ws
éé 
.
éé 
Cell
éé 
(
éé 
$num
éé 
,
éé 
$num
éé 
)
éé 
.
éé 
Value
éé 
=
éé  !
$str
éé" 1
;
éé1 2
ws
èè 
.
èè 
Cell
èè 
(
èè 
$num
èè 
,
èè 
$num
èè 
)
èè 
.
èè 
Value
èè 
=
èè  !
$str
èè" +
;
èè+ ,
ws
êê 
.
êê 
Cell
êê 
(
êê 
$num
êê 
,
êê 
$num
êê 
)
êê 
.
êê 
Value
êê 
=
êê  !
$str
êê" +
;
êê+ ,
ws
ëë 
.
ëë 
Cell
ëë 
(
ëë 
$num
ëë 
,
ëë 
$num
ëë 
)
ëë 
.
ëë 
Value
ëë 
=
ëë  !
$str
ëë" +
;
ëë+ ,
ws
ìì 
.
ìì 
Range
ìì 
(
ìì 
$num
ìì 
,
ìì 
$num
ìì 
,
ìì 
$num
ìì 
,
ìì 
$num
ìì 
)
ìì  
.
ìì  !
Style
ìì! &
.
ìì& '
Font
ìì' +
.
ìì+ ,
Bold
ìì, 0
=
ìì1 2
true
ìì3 7
;
ìì7 8
int
ïï 
row
ïï 
=
ïï 
$num
ïï 
;
ïï 
foreach
ññ 
(
ññ 
var
ññ 
person
ññ 
in
ññ  "
people
ññ# )
)
ññ) *
{
óó 
ws
òò 
.
òò 
Cell
òò 
(
òò 
row
òò 
,
òò 
$num
òò 
)
òò 
.
òò  
Value
òò  %
=
òò& '
person
òò( .
.
òò. /
PersonId
òò/ 7
;
òò7 8
ws
ôô 
.
ôô 
Cell
ôô 
(
ôô 
row
ôô 
,
ôô 
$num
ôô 
)
ôô 
.
ôô  
Value
ôô  %
=
ôô& '
person
ôô( .
.
ôô. /
Person
ôô/ 5
;
ôô5 6
ws
öö 
.
öö 
Cell
öö 
(
öö 
row
öö 
,
öö 
$num
öö 
)
öö 
.
öö  
Value
öö  %
=
öö& '
person
öö( .
.
öö. /

LocationId
öö/ 9
;
öö9 :
ws
õõ 
.
õõ 
Cell
õõ 
(
õõ 
row
õõ 
,
õõ 
$num
õõ 
)
õõ 
.
õõ  
Value
õõ  %
=
õõ& '
person
õõ( .
.
õõ. /
AphaLocation
õõ/ ;
;
õõ; <
ws
úú 
.
úú 
Cell
úú 
(
úú 
row
úú 
,
úú 
$num
úú 
)
úú 
.
úú  
Value
úú  %
=
úú& '
person
úú( .
.
úú. /
Trainer
úú/ 6
;
úú6 7
ws
ùù 
.
ùù 
Cell
ùù 
(
ùù 
row
ùù 
,
ùù 
$num
ùù 
)
ùù 
.
ùù  
Value
ùù  %
=
ùù& '
person
ùù( .
.
ùù. /
Trainee
ùù/ 6
;
ùù6 7
ws
ûû 
.
ûû 
Cell
ûû 
(
ûû 
row
ûû 
,
ûû 
$num
ûû 
)
ûû 
.
ûû  
Value
ûû  %
=
ûû& '
person
ûû( .
.
ûû. /
Trained
ûû/ 6
;
ûû6 7
row
üü 
++
üü 
;
üü 
}
†† 
}
°° 	
private
££ 
static
££ 
void
££ 
FillTrainersSheet
££ -
(
££- .

XLWorkbook
££. 8
workbook
££9 A
,
££A B
List
££C G
<
££G H
TrainerReportDto
££H X
>
££X Y
trainers
££Z b
)
££b c
{
§§ 	
var
•• 
ws
•• 
=
•• 
workbook
•• 
.
•• 

Worksheets
•• (
.
••( )
FirstOrDefault
••) 7
(
••7 8
w
••8 9
=>
••: <
w
••= >
.
••> ?
Name
••? C
==
••D F
$str
••G Q
)
••Q R
??
••S U
workbook
••V ^
.
••^ _

Worksheets
••_ i
.
••i j
Add
••j m
(
••m n
$str
••n x
)
••x y
;
••y z
ws
¶¶ 
.
¶¶ 
Clear
¶¶ 
(
¶¶ 
)
¶¶ 
;
¶¶ 
ws
ßß 
.
ßß 
Cell
ßß 
(
ßß 
$num
ßß 
,
ßß 
$num
ßß 
)
ßß 
.
ßß 
Value
ßß 
=
ßß  !
$str
ßß" .
;
ßß. /
ws
®® 
.
®® 
Cell
®® 
(
®® 
$num
®® 
,
®® 
$num
®® 
)
®® 
.
®® 
Value
®® 
=
®®  !
$str
®®" +
;
®®+ ,
ws
©© 
.
©© 
Cell
©© 
(
©© 
$num
©© 
,
©© 
$num
©© 
)
©© 
.
©© 
Value
©© 
=
©©  !
$str
©©" +
;
©©+ ,
ws
™™ 
.
™™ 
Cell
™™ 
(
™™ 
$num
™™ 
,
™™ 
$num
™™ 
)
™™ 
.
™™ 
Value
™™ 
=
™™  !
$str
™™" -
;
™™- .
ws
´´ 
.
´´ 
Cell
´´ 
(
´´ 
$num
´´ 
,
´´ 
$num
´´ 
)
´´ 
.
´´ 
Value
´´ 
=
´´  !
$str
´´" )
;
´´) *
ws
¨¨ 
.
¨¨ 
Range
¨¨ 
(
¨¨ 
$num
¨¨ 
,
¨¨ 
$num
¨¨ 
,
¨¨ 
$num
¨¨ 
,
¨¨ 
$num
¨¨ 
)
¨¨  
.
¨¨  !
Style
¨¨! &
.
¨¨& '
Font
¨¨' +
.
¨¨+ ,
Bold
¨¨, 0
=
¨¨1 2
true
¨¨3 7
;
¨¨7 8
int
ÆÆ 
row
ÆÆ 
=
ÆÆ 
$num
ÆÆ 
;
ÆÆ 
foreach
ØØ 
(
ØØ 
var
ØØ 
trainer
ØØ  
in
ØØ! #
trainers
ØØ$ ,
)
ØØ, -
{
∞∞ 
ws
±± 
.
±± 
Cell
±± 
(
±± 
row
±± 
,
±± 
$num
±± 
)
±± 
.
±±  
Value
±±  %
=
±±& '
trainer
±±( /
.
±±/ 0
ID
±±0 2
;
±±2 3
var
≤≤ 
idCell
≤≤ 
=
≤≤ 
ws
≤≤ 
.
≤≤  
Cell
≤≤  $
(
≤≤$ %
row
≤≤% (
,
≤≤( )
$num
≤≤* +
)
≤≤+ ,
;
≤≤, -
idCell
≥≥ 
.
≥≥ 
Value
≥≥ 
=
≥≥ 
trainer
≥≥ &
.
≥≥& '
Trainer
≥≥' .
;
≥≥. /
idCell
¥¥ 
.
¥¥ 
SetHyperlink
¥¥ #
(
¥¥# $
new
¥¥$ '
XLHyperlink
¥¥( 3
(
¥¥3 4
$str
¥¥4 C
)
¥¥C D
)
¥¥D E
;
¥¥E F
ws
µµ 
.
µµ 
Cell
µµ 
(
µµ 
row
µµ 
,
µµ 
$num
µµ 
)
µµ 
.
µµ  
Value
µµ  %
=
µµ& '
trainer
µµ( /
.
µµ/ 0
Trained
µµ0 7
;
µµ7 8
ws
∂∂ 
.
∂∂ 
Cell
∂∂ 
(
∂∂ 
row
∂∂ 
,
∂∂ 
$num
∂∂ 
)
∂∂ 
.
∂∂  
Value
∂∂  %
=
∂∂& '
trainer
∂∂( /
.
∂∂/ 0
RunTot
∂∂0 6
;
∂∂6 7
ws
∑∑ 
.
∑∑ 
Cell
∑∑ 
(
∑∑ 
row
∑∑ 
,
∑∑ 
$num
∑∑ 
)
∑∑ 
.
∑∑  
Value
∑∑  %
=
∑∑& '
trainer
∑∑( /
.
∑∑/ 0
Excel
∑∑0 5
;
∑∑5 6
row
∏∏ 
++
∏∏ 
;
∏∏ 
}
ππ 
}
∫∫ 	
private
ºº 
static
ºº 
void
ºº 
FillTrainingSheet
ºº -
(
ºº- .

XLWorkbook
ºº. 8
workbook
ºº9 A
,
ººA B
List
ººC G
<
ººG H
TrainingReportDto
ººH Y
>
ººY Z
	trainings
ºº[ d
)
ººd e
{
ΩΩ 	
var
ææ 
ws
ææ 
=
ææ 
workbook
ææ 
.
ææ 

Worksheets
ææ (
.
ææ( )
FirstOrDefault
ææ) 7
(
ææ7 8
w
ææ8 9
=>
ææ: <
w
ææ= >
.
ææ> ?
Name
ææ? C
==
ææD F
$str
ææG Q
)
ææQ R
??
ææS U
workbook
ææV ^
.
ææ^ _

Worksheets
ææ_ i
.
ææi j
Add
ææj m
(
ææm n
$str
ææn x
)
ææx y
;
ææy z
ws
øø 
.
øø 
Clear
øø 
(
øø 
)
øø 
;
øø 
ws
¿¿ 
.
¿¿ 
Cell
¿¿ 
(
¿¿ 
$num
¿¿ 
,
¿¿ 
$num
¿¿ 
)
¿¿ 
.
¿¿ 
Value
¿¿ 
=
¿¿  !
$str
¿¿" +
;
¿¿+ ,
ws
¡¡ 
.
¡¡ 
Cell
¡¡ 
(
¡¡ 
$num
¡¡ 
,
¡¡ 
$num
¡¡ 
)
¡¡ 
.
¡¡ 
Value
¡¡ 
=
¡¡  !
$str
¡¡" +
;
¡¡+ ,
ws
¬¬ 
.
¬¬ 
Cell
¬¬ 
(
¬¬ 
$num
¬¬ 
,
¬¬ 
$num
¬¬ 
)
¬¬ 
.
¬¬ 
Value
¬¬ 
=
¬¬  !
$str
¬¬" 0
;
¬¬0 1
ws
√√ 
.
√√ 
Cell
√√ 
(
√√ 
$num
√√ 
,
√√ 
$num
√√ 
)
√√ 
.
√√ 
Value
√√ 
=
√√  !
$str
√√" +
;
√√+ ,
ws
ƒƒ 
.
ƒƒ 
Cell
ƒƒ 
(
ƒƒ 
$num
ƒƒ 
,
ƒƒ 
$num
ƒƒ 
)
ƒƒ 
.
ƒƒ 
Value
ƒƒ 
=
ƒƒ  !
$str
ƒƒ" '
;
ƒƒ' (
ws
≈≈ 
.
≈≈ 
Range
≈≈ 
(
≈≈ 
$num
≈≈ 
,
≈≈ 
$num
≈≈ 
,
≈≈ 
$num
≈≈ 
,
≈≈ 
$num
≈≈ 
)
≈≈  
.
≈≈  !
Style
≈≈! &
.
≈≈& '
Font
≈≈' +
.
≈≈+ ,
Bold
≈≈, 0
=
≈≈1 2
true
≈≈3 7
;
≈≈7 8
int
«« 
row
«« 
=
«« 
$num
«« 
;
«« 
foreach
»» 
(
»» 
var
»» 
training
»» !
in
»»" $
	trainings
»»% .
)
»». /
{
…… 
ws
   
.
   
Cell
   
(
   
row
   
,
   
$num
   
)
   
.
    
Value
    %
=
  & '
training
  ( 0
.
  0 1
Trainer
  1 8
;
  8 9
ws
ÀÀ 
.
ÀÀ 
Cell
ÀÀ 
(
ÀÀ 
row
ÀÀ 
,
ÀÀ 
$num
ÀÀ 
)
ÀÀ 
.
ÀÀ  
Value
ÀÀ  %
=
ÀÀ& '
training
ÀÀ( 0
.
ÀÀ0 1
Trainee
ÀÀ1 8
;
ÀÀ8 9
ws
ÃÃ 
.
ÃÃ 
Cell
ÃÃ 
(
ÃÃ 
row
ÃÃ 
,
ÃÃ 
$num
ÃÃ 
)
ÃÃ 
.
ÃÃ  
Value
ÃÃ  %
=
ÃÃ& '
training
ÃÃ( 0
.
ÃÃ0 1
	TrainedOn
ÃÃ1 :
;
ÃÃ: ;
ws
ÕÕ 
.
ÕÕ 
Cell
ÕÕ 
(
ÕÕ 
row
ÕÕ 
,
ÕÕ 
$num
ÕÕ 
)
ÕÕ 
.
ÕÕ  
Value
ÕÕ  %
=
ÕÕ& '
training
ÕÕ( 0
.
ÕÕ0 1
TrainingAnimal
ÕÕ1 ?
;
ÕÕ? @
ws
ŒŒ 
.
ŒŒ 
Cell
ŒŒ 
(
ŒŒ 
row
ŒŒ 
,
ŒŒ 
$num
ŒŒ 
)
ŒŒ 
.
ŒŒ  
Value
ŒŒ  %
=
ŒŒ& '
training
ŒŒ( 0
.
ŒŒ0 1
VLA
ŒŒ1 4
;
ŒŒ4 5
row
œœ 
++
œœ 
;
œœ 
}
–– 
}
—— 	
private
”” 
static
”” 
void
””  
FillLocationsSheet
”” .
(
””. /

XLWorkbook
””/ 9
workbook
””: B
,
””B C
List
””D H
<
””H I
AphaReportDto
””I V
>
””V W
	locations
””X a
)
””a b
{
‘‘ 	
var
’’ 
ws
’’ 
=
’’ 
workbook
’’ 
.
’’ 

Worksheets
’’ (
.
’’( )
FirstOrDefault
’’) 7
(
’’7 8
w
’’8 9
=>
’’: <
w
’’= >
.
’’> ?
Name
’’? C
==
’’D F
$str
’’G R
)
’’R S
??
’’T V
workbook
’’W _
.
’’_ `

Worksheets
’’` j
.
’’j k
Add
’’k n
(
’’n o
$str
’’o u
)
’’u v
;
’’v w
ws
÷÷ 
.
÷÷ 
Clear
÷÷ 
(
÷÷ 
)
÷÷ 
;
÷÷ 
ws
◊◊ 
.
◊◊ 
Cell
◊◊ 
(
◊◊ 
$num
◊◊ 
,
◊◊ 
$num
◊◊ 
)
◊◊ 
.
◊◊ 
Value
◊◊ 
=
◊◊  !
$str
◊◊" 0
;
◊◊0 1
ws
ÿÿ 
.
ÿÿ 
Cell
ÿÿ 
(
ÿÿ 
$num
ÿÿ 
,
ÿÿ 
$num
ÿÿ 
)
ÿÿ 
.
ÿÿ 
Value
ÿÿ 
=
ÿÿ  !
$str
ÿÿ" ,
;
ÿÿ, -
ws
ŸŸ 
.
ŸŸ 
Cell
ŸŸ 
(
ŸŸ 
$num
ŸŸ 
,
ŸŸ 
$num
ŸŸ 
)
ŸŸ 
.
ŸŸ 
Value
ŸŸ 
=
ŸŸ  !
$str
ŸŸ" (
;
ŸŸ( )
ws
⁄⁄ 
.
⁄⁄ 
Range
⁄⁄ 
(
⁄⁄ 
$num
⁄⁄ 
,
⁄⁄ 
$num
⁄⁄ 
,
⁄⁄ 
$num
⁄⁄ 
,
⁄⁄ 
$num
⁄⁄ 
)
⁄⁄  
.
⁄⁄  !
Style
⁄⁄! &
.
⁄⁄& '
Font
⁄⁄' +
.
⁄⁄+ ,
Bold
⁄⁄, 0
=
⁄⁄1 2
true
⁄⁄3 7
;
⁄⁄7 8
int
‹‹ 
row
‹‹ 
=
‹‹ 
$num
‹‹ 
;
‹‹ 
foreach
›› 
(
›› 
var
›› 
location
›› !
in
››" $
	locations
››% .
)
››. /
{
ﬁﬁ 
ws
ﬂﬂ 
.
ﬂﬂ 
Cell
ﬂﬂ 
(
ﬂﬂ 
row
ﬂﬂ 
,
ﬂﬂ 
$num
ﬂﬂ 
)
ﬂﬂ 
.
ﬂﬂ  
Value
ﬂﬂ  %
=
ﬂﬂ& '
location
ﬂﬂ( 0
.
ﬂﬂ0 1
ID
ﬂﬂ1 3
;
ﬂﬂ3 4
ws
‡‡ 
.
‡‡ 
Cell
‡‡ 
(
‡‡ 
row
‡‡ 
,
‡‡ 
$num
‡‡ 
)
‡‡ 
.
‡‡  
Value
‡‡  %
=
‡‡& '
location
‡‡( 0
.
‡‡0 1
Location
‡‡1 9
;
‡‡9 :
ws
·· 
.
·· 
Cell
·· 
(
·· 
row
·· 
,
·· 
$num
·· 
)
·· 
.
··  
Value
··  %
=
··& '
location
··( 0
.
··0 1
APHA
··1 5
;
··5 6
row
‚‚ 
++
‚‚ 
;
‚‚ 
}
„„ 
}
‰‰ 	
}
ÁÁ 
}ËË ∏>
SD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\PersonsService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class 
PersonsService 
:  !
IPersonsService" 1
{ 
private 
readonly 
IPersonsRepository +
_personRepository, =
;= >
private 
readonly 
IMapper  
_mapper! (
;( )
public 
PersonsService 
( 
IPersonsRepository 0
personRepository1 A
,A B
IMapperC J
mapperK Q
)Q R
{ 	
_personRepository 
= 
personRepository  0
??1 3
throw4 9
new: =!
ArgumentNullException> S
(S T
nameofT Z
(Z [
personRepository[ k
)k l
)l m
;m n
_mapper 
= 
mapper 
?? 
throw  %
new& )!
ArgumentNullException* ?
(? @
nameof@ F
(F G
mapperG M
)M N
)N O
;O P
} 	
public 
async 
Task 
< 
IEnumerable %
<% &
PersonLookupDto& 5
>5 6
>6 7&
GetPersonsForDropdownAsync8 R
(R S
)S T
{ 	
var 
entities 
= 
await  
_personRepository! 2
.2 3)
GetAllPersonsForDropdownAsync3 P
(P Q
)Q R
;R S
return 
_mapper 
. 
Map 
< 
IEnumerable *
<* +
PersonLookupDto+ :
>: ;
>; <
(< =
entities= E
)E F
;F G
} 	
public 
async 
Task 
< 
string  
?  !
>! "
GetSiteByIdAsync# 3
(3 4
int4 7
personId8 @
)@ A
{ 	
var 
site 
= 
await 
_personRepository .
.. /
GetSiteByIdAsync/ ?
(? @
personId@ H
)H I
;I J
return 
site 
; 
}!! 	
public## 
async## 
Task## 
<## 
IEnumerable## %
<##% &
PersonSiteLookupDto##& 9
>##9 :
>##: ;
GetAllSitesAsync##< L
(##L M
string##M S
plantNo##T [
)##[ \
{$$ 	
var%% 
sites%% 
=%% 
await%% 
_personRepository%% /
.%%/ 0
GetAllSitesAsync%%0 @
(%%@ A
plantNo%%A H
)%%H I
;%%I J
return&& 
_mapper&& 
.&& 
Map&& 
<&& 
IEnumerable&& *
<&&* +
PersonSiteLookupDto&&+ >
>&&> ?
>&&? @
(&&@ A
sites&&A F
)&&F G
;&&G H
}'' 	
public(( 
async(( 
Task(( 
<(( 
IEnumerable(( %
<((% &
PersonDetailDto((& 5
>((5 6
>((6 7#
GetAllPersonByNameAsync((8 O
(((O P
int((P S
personId((T \
)((\ ]
{)) 	
var** 
personsData** 
=** 
await** "
_personRepository**# 4
.**4 5#
GetAllPersonByNameAsync**5 L
(**L M
personId**M U
)**U V
;**V W
var++ 
result++ 
=++ 
_mapper++ 
.++  
Map++  #
<++# $
IEnumerable++$ /
<++/ 0
PersonDetailDto++0 ?
>++? @
>++@ A
(++A B
personsData++B M
)++M N
;++N O
return,, 
result,, 
;,, 
}-- 	
public.. 
async.. 
Task.. 
<.. 
string..  
>..  !
AddPersonAsync.." 0
(..0 1
AddPersonDto..1 =

personsDto..> H
,..H I
string..J P
userName..Q Y
)..Y Z
{// 	
var00 
person00 
=00 
_mapper00  
.00  !
Map00! $
<00$ %
	AddPerson00% .
>00. /
(00/ 0

personsDto000 :
)00: ;
;00; <
await11 
_personRepository11 #
.11# $
AddPersonAsync11$ 2
(112 3
person113 9
,119 :
userName11; C
)11C D
;11D E
return22 
$"22 
{22 

personsDto22  
.22  !
Name22! %
}22% &
$str22& 8
"228 9
;229 :
}44 	
public55 
async55 
Task55 
<55 
string55  
?55  !
>55! ""
GetPersonNameByIdAsync55# 9
(559 :
int55: =
personId55> F
)55F G
{66 	
var88 
person88 
=88 
await88 
_personRepository88 0
.880 1"
GetPersonNameByIdAsync881 G
(88G H
personId88H P
)88P Q
;88Q R
return99 
person99 
;99 
};; 	
public<< 
async<< 
Task<< 
<<< 
string<<  
><<  !
UpdatePersonAsync<<" 3
(<<3 4
EditPersonDto<<4 A
dto<<B E
)<<E F
{== 	
var?? 

editPerson?? 
=?? 
_mapper?? $
.??$ %
Map??% (
<??( )

EditPerson??) 3
>??3 4
(??4 5
dto??5 8
)??8 9
;??9 :
stringAA 
traineeNameAA 
=AA  
dtoAA! $
.AA$ %
PersonAA% +
;AA+ ,
awaitCC 
_personRepositoryCC #
.CC# $
UpdatePersonAsyncCC$ 5
(CC5 6

editPersonCC6 @
)CC@ A
;CCA B
returnEE 
$"EE 
{EE 
traineeNameEE !
}EE! "
$strEE" :
"EE: ;
;EE; <
}GG 	
publicHH 
asyncHH 
TaskHH 
<HH 
stringHH  
>HH  !
DeletePersonAsyncHH" 3
(HH3 4
intHH4 7
personIdHH8 @
)HH@ A
{II 	
varKK 

personNameKK 
=KK 
awaitKK "
_personRepositoryKK# 4
.KK4 5"
GetPersonNameByIdAsyncKK5 K
(KKK L
personIdKKL T
)KKT U
;KKU V
varMM 
siteMM 
=MM 
awaitMM 
_personRepositoryMM .
.MM. /
GetSiteByIdAsyncMM/ ?
(MM? @
personIdMM@ H
)MMH I
;MMI J
varOO 
deletedOO 
=OO 
awaitOO 
_personRepositoryOO  1
.OO1 2
DeletePersonAsyncOO2 C
(OOC D
personIdOOD L
)OOL M
;OOM N
ifQQ 
(QQ 
deletedQQ 
)QQ 
{RR 
returnSS 
$"SS 
{SS 

personNameSS $
}SS$ %
$strSS% +
{SS+ ,
siteSS, 0
}SS0 1
$strSS1 U
"SSU V
;SSV W
}TT 
elseUU 
{VV 
returnWW 
$"WW 
{WW 

personNameWW $
}WW$ %
$strWW% o
"WWo p
;WWp q
}XX 
}YY 	
}[[ 
}\\ ñ
PD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\NewsService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class 
NewsService 
: 
INewsService +
{ 
private 
readonly 
INewsRepository (
_newsRepository) 8
;8 9
private 
readonly 
IMapper  
_mapper! (
;( )
public 
NewsService 
( 
INewsRepository *
newseRepository+ :
,: ;
IMapper< C
mapperD J
)J K
{ 	
_newsRepository 
= 
newseRepository -
??. 0
throw1 6
new7 :!
ArgumentNullException; P
(P Q
nameofQ W
(W X
newseRepositoryX g
)g h
)h i
;i j
_mapper 
= 
mapper 
; 
} 	
public 
async 
Task 
< 
IEnumerable %
<% &
NewsDto& -
>- .
>. /
GetLatestNewsAsync0 B
(B C
)C D
{ 	
var 

latestNews 
= 
await "
_newsRepository# 2
.2 3
GetLatestNewsAsync3 E
(E F
)F G
;G H
return 
_mapper 
. 
Map 
< 
IEnumerable *
<* +
NewsDto+ 2
>2 3
>3 4
(4 5

latestNews5 ?
)? @
;@ A
} 	
public 
async 
Task 
< 
string  
>  !
AddNewsAsync" .
(. /
NewsDto/ 6
dto7 :
): ;
{ 	
var   
news   
=   
_mapper   
.   
Map   "
<  " #
News  # '
>  ' (
(  ( )
dto  ) ,
)  , -
;  - .
await!! 
_newsRepository!! !
.!!! "
AddNewsAsync!!" .
(!!. /
news!!/ 3
)!!3 4
;!!4 5
return"" 
$""" 
$str""  
{""  !
dto""! $
.""$ %
Title""% *
}""* +
$str""+ F
"""F G
;""G H
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% 
NewsDto%% &
>%%& '
>%%' (
GetNewsAsync%%) 5
(%%5 6
)%%6 7
{&& 	
var'' 
news'' 
='' 
await'' 
_newsRepository'' ,
.'', -
GetNewsAsync''- 9
(''9 :
)'': ;
;''; <
return(( 
_mapper(( 
.(( 
Map(( 
<(( 
List(( #
<((# $
NewsDto(($ +
>((+ ,
>((, -
(((- .
news((. 2
)((2 3
;((3 4
})) 	
public++ 
async++ 
Task++ 
<++ 
string++  
>++  !
DeleteNewsAsync++" 1
(++1 2
string++2 8
title++9 >
)++> ?
{,, 	
await-- 
_newsRepository-- !
.--! "
DeleteNewsAsync--" 1
(--1 2
title--2 7
)--7 8
;--8 9
return.. 
$".. 
$str.. (
{..( )
title..) .
}... /
$str../ @
"..@ A
;..A B
}// 	
}00 
}11 ﬂ
UD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\DataEntryService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class 
DataEntryService !
:! "
IDataEntryService" 3
{ 
private 
readonly  
IDataEntryRepository - 
_dataEntryRepository. B
;B C
public 
DataEntryService 
(   
IDataEntryRepository  4
dataEntryRepository5 H
)H I
{ 	 
_dataEntryRepository  
=! "
dataEntryRepository# 6
??7 9
throw: ?
new@ C!
ArgumentNullExceptionD Y
(Y Z
nameofZ `
(` a
dataEntryRepositorya t
)t u
)u v
;v w
} 	
public 
async 
Task 
< 
bool 
> 
CanEditPage  +
(+ ,
string, 2
action3 9
)9 :
{ 	
var 
canEdit 
= 
await  
_dataEntryRepository  4
.4 5
CanEditPage5 @
(@ A
actionA G
)G H
;H I
return 
canEdit 
; 
} 	
} 
} ¬
TD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\AuditLogService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class 
AuditLogService  
:! "
IAuditLogService# 3
{ 
private 
readonly 
IAuditLogRepository ,
_auditLogRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
public 
AuditLogService 
( 
IAuditLogRepository 2
auditLogRepository3 E
,E F
IMapperG N
mapperO U
)U V
{ 	
_auditLogRepository 
=  !
auditLogRepository" 4
;4 5
_mapper 
= 
mapper 
?? 
throw  %
new& )!
ArgumentNullException* ?
(? @
nameof@ F
(F G
mapperG M
)M N
)N O
;O P
} 	
public 
async 
Task 
< 
PaginatedResult )
<) *
AuditLogDto* 5
>5 6
>6 7
GetAuditLogsAsync8 I
(I J
QueryParametersJ Y
filterZ `
,` a
stringb h
storedProcedurei x
)x y
{ 	
var 
queryFilter 
= 
_mapper %
.% &
Map& )
<) * 
PaginationParameters* >
>> ?
(? @
filter@ F
)F G
;G H
var 
	auditLogs 
= 
await !
_auditLogRepository" 5
.5 6
GetAuditLogsAsync6 G
(G H
queryFilterH S
,S T
storedProcedureU d
)d e
;e f
return 
_mapper 
. 
Map 
< 
PaginatedResult .
<. /
AuditLogDto/ :
>: ;
>; <
(< =
	auditLogs= F
)F G
;G H
}!! 	
public"" 
async"" 
Task"" 
<"" 
List"" 
<"" 
string"" %
>""% &
>""& '(
GetStoredProcedureNamesAsync""( D
(""D E
)""E F
{## 	
return$$ 
await$$ 
_auditLogRepository$$ ,
.$$, -(
GetStoredProcedureNamesAsync$$- I
($$I J
)$$J K
;$$K L
}%% 	
public&& 
async&& 
Task&&  
ArchiveAuditLogAsync&& .
(&&. /
string&&/ 5
userName&&6 >
)&&> ?
{'' 	
await(( 
_auditLogRepository(( %
.((% & 
ArchiveAuditLogAsync((& :
(((: ;
userName((; C
)((C D
;((D E
})) 	
}** 
}++ ‘
\D:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\AuditlogArchivedService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class #
AuditlogArchivedService (
:) *$
IAuditlogArchivedService* B
{ 
private 
readonly '
IAuditlogArchivedRepository 4&
_auditLogArchiveRepository5 O
;O P
private 
readonly 
IMapper  
_mapper! (
;( )
public #
AuditlogArchivedService &
(& ''
IAuditlogArchivedRepository' B%
auditLogArchiveRepositoryC \
,\ ]
IMapper^ e
mapperf l
)l m
{ 	&
_auditLogArchiveRepository &
=' (%
auditLogArchiveRepository) B
;B C
_mapper 
= 
mapper 
?? 
throw  %
new& )!
ArgumentNullException* ?
(? @
nameof@ F
(F G
mapperG M
)M N
)N O
;O P
} 	
public 
async 
Task 
< 
PaginatedResult )
<) *
AuditLogArchivedDto* =
>= >
>> ?$
GetArchiveAuditLogsAsync@ X
(X Y
QueryParametersY h
filteri o
,o p
stringq w
storedProcedure	x á
)
á à
{ 	
var 
queryFilter 
= 
_mapper %
.% &
Map& )
<) * 
PaginationParameters* >
>> ?
(? @
filter@ F
)F G
;G H
var 
	auditLogs 
= 
await !&
_auditLogArchiveRepository" <
.< =$
GetArchiveAuditLogsAsync= U
(U V
queryFilterV a
,a b
storedProcedurec r
)r s
;s t
return 
_mapper 
. 
Map 
< 
PaginatedResult .
<. /
AuditLogArchivedDto/ B
>B C
>C D
(D E
	auditLogsE N
)N O
;O P
} 	
public 
async 
Task 
< 
List 
< 
string %
>% &
>& '(
GetStoredProcedureNamesAsync( D
(D E
)E F
{   	
return!! 
await!! &
_auditLogArchiveRepository!! 3
.!!3 4(
GetStoredProcedureNamesAsync!!4 P
(!!P Q
)!!Q R
;!!R S
}"" 	
}## 
}$$ ÿ
YD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Services\AccessControlService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Services '
{ 
public 

class  
AccessControlService %
:& '!
IAccessControlService( =
{ 
private 
readonly $
IAccessControlRepository 1$
_AccessControlRepository2 J
;J K
public  
AccessControlService #
(# $$
IAccessControlRepository$ <#
accessControlRepository= T
)T U
{ 	$
_AccessControlRepository $
=% &#
accessControlRepository' >
??? A
throwB G
newH K!
ArgumentNullExceptionL a
(a b
nameofb h
(h i$
accessControlRepository	i Ä
)
Ä Å
)
Å Ç
;
Ç É
} 	
public 
async 
Task 
< 
( 
byte 
?  
RoleId! '
,' (
string) /
?/ 0
Username1 9
)9 :
?: ;
>; <,
 GetRoleIdAndUsernameByEmailAsync= ]
(] ^
string^ d
emaile j
)j k
{ 	
var 
result 
= 
await $
_AccessControlRepository 7
.7 8,
 GetRoleIdAndUsernameByEmailAsync8 X
(X Y
emailY ^
)^ _
;_ `
if 
( 
result 
!= 
null 
) 
{ 
return 
( 
result 
. 
Value $
.$ %
RoleId% +
,+ ,
result- 3
.3 4
Value4 9
.9 :
Username: B
)B C
;C D
} 
return 
null 
; 
} 	
} 
} Ì
VD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Pagination\QueryParameters.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Pagination )
{ 
public 

class 
QueryParameters  
{ 
private 
int 
	_pageSize 
= 
$num  "
;" #
private 
int 
_page 
= 
$num 
; 
private 
const 
int 
MaxPageSize %
=& '
$num( +
;+ ,
public		 
string		 
?		 
Search		 
{		 
get		  #
;		# $
set		% (
;		( )
}		* +
public

 
string

 
?

 
SortBy

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
=

, -
$str

. 0
;

0 1
public 
bool 

Descending 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
false/ 4
;4 5
public 
int 
Page 
{ 	
get 
=> 
_page 
; 
set 
=> 
_page 
= 
value  
<! "
$num# $
?% &
$num' (
:) *
value+ 0
;0 1
} 	
public 
int 
PageSize 
{ 	
get 
=> 
	_pageSize 
; 
set 
=> 
	_pageSize 
= 
value $
>% &
MaxPageSize' 2
?3 4
MaxPageSize5 @
:A B
valueC H
;H I
} 	
} 
} õ
VD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Pagination\PaginatedResult.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Pagination )
{ 
public 

class 
PaginatedResult  
<  !
T! "
>" #
{ 
public 
IEnumerable 
< 
T 
> 
data "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2

Enumerable3 =
.= >
Empty> C
<C D
TD E
>E F
(F G
)G H
;H I
public 
int 

TotalCount 
{ 
get  #
;# $
set% (
;( )
}* +
public 
PaginatedResult 
( 
)  
{! "
}# $
public

 
PaginatedResult

 
(

 
IEnumerable

 *
<

* +
T

+ ,
>

, -
items

. 3
,

3 4
int

5 8

totalCount

9 C
)

C D
{ 	
data 
= 
items 
; 

TotalCount 
= 

totalCount #
;# $
} 	
} 
} „=
QD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Mappings\EntityMapper.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
Mappings '
{ 
public		 

class		 
EntityMapper		 
:		 
Profile		  '
{

 
public 
EntityMapper 
( 
) 
{ 	
	CreateMap 
< 
Persons 
, 

PersonsDto )
>) *
(* +
)+ ,
., -

ReverseMap- 7
(7 8
)8 9
;9 :
	CreateMap 
< 
Site 
, 
SiteDto #
># $
($ %
)% &
.& '

ReverseMap' 1
(1 2
)2 3
;3 4
	CreateMap 
< 
SiteTrainee !
,! "
SiteTraineeDto# 1
>1 2
(2 3
)3 4
.4 5

ReverseMap5 ?
(? @
)@ A
;A B
	CreateMap 
< 
TrainerTrainingDto (
,( )
TrainerTraining* 9
>9 :
(: ;
); <
.< =

ReverseMap= G
(G H
)H I
;I J
	CreateMap 
< 
TraineeTrainer $
,$ %

TraineeDto& 0
>0 1
(1 2
)2 3
.3 4

ReverseMap4 >
(> ?
)? @
;@ A
	CreateMap 
< 
TrainingDto !
,! "
Training# +
>+ ,
(, -
)- .
.. /

ReverseMap/ 9
(9 :
): ;
;; <
	CreateMap 
< 
EditTrainingDto %
,% &
EditTraining' 3
>3 4
(4 5
)5 6
.6 7

ReverseMap7 A
(A B
)B C
;C D
	CreateMap 
< 
TrainerHistoryDto '
,' (
TrainerHistory) 7
>7 8
(8 9
)9 :
.: ;

ReverseMap; E
(E F
)F G
;G H
	CreateMap 
< 
TrainerTrainedDto '
,' (
TrainerTrained) 7
>7 8
(8 9
)9 :
.: ;

ReverseMap; E
(E F
)F G
;G H
	CreateMap 
< 
NewsDto 
, 
News #
># $
($ %
)% &
.& '

ReverseMap' 1
(1 2
)2 3
;3 4
	CreateMap 
< 
AddPersonDto "
," #
	AddPerson$ -
>- .
(. /
)/ 0
.0 1

ReverseMap1 ;
(; <
)< =
;= >
	CreateMap 
< 
EditPersonDto #
,# $

EditPerson% /
>/ 0
(0 1
)1 2
.2 3

ReverseMap3 =
(= >
)> ?
;? @
	CreateMap 
< 
PersonSiteLookupDto )
,) *
PersonSiteLookup+ ;
>; <
(< =
)= >
.> ?

ReverseMap? I
(I J
)J K
;K L
	CreateMap 
< 
PersonLookupDto %
,% &
PersonLookup' 3
>3 4
(4 5
)5 6
.6 7

ReverseMap7 A
(A B
)B C
;C D
	CreateMap 
< 
PersonDetailDto %
,% &
PersonDetail' 3
>3 4
(4 5
)5 6
.6 7

ReverseMap7 A
(A B
)B C
;C D
	CreateMap 
< 
UserDto 
, 
User #
># $
($ %
)% &
.& '

ReverseMap' 1
(1 2
)2 3
;3 4
	CreateMap 
< 
UserView 
, 
UserViewDto  +
>+ ,
(, -
)- .
.. /

ReverseMap/ 9
(9 :
): ;
;; <
	CreateMap 
< 
	VlaLocDto 
,  
VlaLoc! '
>' (
(( )
)) *
.* +

ReverseMap+ 5
(5 6
)6 7
;7 8
	CreateMap 
< 

VlaLocView  
,  !
	VlaLocDto" +
>+ ,
(, -
)- .
.. /

ReverseMap/ 9
(9 :
): ;
;; <
	CreateMap   
<   
	SiteInput   
,    
SiteInputDto  ! -
>  - .
(  . /
)  / 0
.  0 1

ReverseMap  1 ;
(  ; <
)  < =
;  = >
	CreateMap!! 
<!! 

SiteReport!!  
,!!  !
SiteReportDto!!" /
>!!/ 0
(!!0 1
)!!1 2
.!!2 3

ReverseMap!!3 =
(!!= >
)!!> ?
;!!? @
	CreateMap"" 
<"" 
TrainerReport"" #
,""# $
TrainerReportDto""% 5
>""5 6
(""6 7
)""7 8
.""8 9

ReverseMap""9 C
(""C D
)""D E
;""E F
	CreateMap$$ 
<$$ 
PeopleReport$$ "
,$$" #
PeopleReportDto$$$ 3
>$$3 4
($$4 5
)$$5 6
.%% 
	ForMember%% 
(%% 
dest%% 
=>%%  "
dest%%# '
.%%' (

LocationId%%( 2
,%%2 3
opt%%4 7
=>%%8 :
opt%%; >
.%%> ?
MapFrom%%? F
(%%F G
src%%G J
=>%%K M
src%%N Q
.%%Q R
Name%%R V
)%%V W
)%%W X
;%%X Y
	CreateMap'' 
<'' 
TrainingReport'' $
,''$ %
TrainingReportDto''& 7
>''7 8
(''8 9
)''9 :
.'': ;

ReverseMap''; E
(''E F
)''F G
;''G H
	CreateMap(( 
<(( 

AphaReport((  
,((  !
AphaReportDto((" /
>((/ 0
(((0 1
)((1 2
.((2 3

ReverseMap((3 =
(((= >
)((> ?
;((? @
	CreateMap)) 
<)) 
AuditLog)) 
,)) 
AuditLogDto))  +
>))+ ,
()), -
)))- .
.)). /

ReverseMap))/ 9
())9 :
))): ;
;)); <
	CreateMap** 
<** 
QueryParameters** %
,**% & 
PaginationParameters**' ;
>**; <
(**< =
)**= >
.**> ?

ReverseMap**? I
(**I J
)**J K
;**K L
	CreateMap++ 
<++ 
	PagedData++ 
<++  
AuditLog++  (
>++( )
,++) *
PaginatedResult+++ :
<++: ;
AuditLogDto++; F
>++F G
>++G H
(++H I
)++I J
;++J K
	CreateMap,, 
<,, 
AuditlogArchived,, &
,,,& '
AuditLogArchivedDto,,( ;
>,,; <
(,,< =
),,= >
.,,> ?

ReverseMap,,? I
(,,I J
),,J K
;,,K L
	CreateMap-- 
<-- 
	PagedData-- 
<--  
AuditlogArchived--  0
>--0 1
,--1 2
PaginatedResult--3 B
<--B C
AuditLogArchivedDto--C V
>--V W
>--W X
(--X Y
)--Y Z
;--Z [
}.. 	
}// 
}00 Ï
UD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IVlaLocService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{ 
public		 

	interface		 
IVlaLocService		 #
{

 
} 
} ˆ
SD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IUserService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{		 
public

 

	interface

 
IUserService

 !
{ 
Task 
< 
string 
> 
AddUserAsync !
(! "
UserDto" )
dto* -
)- .
;. /
Task 
< 
List 
< 
	VlaLocDto 
> 
> 
GetLocationsAsync /
(/ 0
)0 1
;1 2
Task 
< 
List 
< 
UserViewDto 
> 
> 
GetUsersAsync  -
(- .
string. 4
userId5 ;
); <
;< =
Task 
< 
string 
> 
DeleteUserAsync $
($ %
string% +
userId, 2
)2 3
;3 4
Task 
< 
string 
> 
UpdateUserAsync $
($ %
UserDto% ,
dto- 0
)0 1
;1 2
Task 
< 
UserDto 
? 
> 
GetUserByIdAsync '
(' (
string( .
userId/ 5
)5 6
;6 7
} 
} ∏
WD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\ITrainingService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{		 
public

 

	interface

 
ITrainingService

 %
{ 
Task 
< 
List 
< 

TraineeDto 
> 
> 
GetTraineesAsync /
(/ 0
)0 1
;1 2
Task 
< 
TrainingDto 
? 
> "
GetTrainingByKeysAsync 1
(1 2
int2 5
	traineeId6 ?
,? @
intA D
	trainerIdE N
,N O
stringP V
speciesW ^
,^ _
DateTime` h
dateTrainedi t
,t u
stringv |
trainingType	} â
)
â ä
;
ä ã
Task 
< 
string 
> 
UpdateTrainingAsync (
(( )
EditTrainingDto) 8
dto9 <
)< =
;= >
Task 
< 
IEnumerable 
< 
TrainerHistoryDto *
>* +
>+ ,"
GetTrainerHistoryAsync- C
(C D
intD G
personIdH P
,P Q
stringR X

animalTypeY c
)c d
;d e
Task 
< 
IEnumerable 
< 
TrainerTrainedDto *
>* +
>+ ,"
GetTrainerTrainedAsync- C
(C D
intD G
	trainerIdH Q
)Q R
;R S
Task 
< 
string 
> 
AddTrainingAsync %
(% &
TrainingDto& 1
trainingDto2 =
)= >
;> ?
Task 
< 
IEnumerable 
< 
TrainerTrainingDto +
>+ ,
>, -%
GetTrainingByTraineeAsync. G
(G H
stringH N
	traineeIdO X
)X Y
;Y Z
Task 
< 
IEnumerable 
< 
TrainerTrainingDto +
>+ ,
>, - 
GetAllTrainingsAsync. B
(B C
)C D
;D E
Task 
< 
string 
> 
DeleteTrainingAsync (
(( )
int) ,
	traineeId- 6
,6 7
string8 >
species? F
,F G
DateTimeH P
dateTrainedQ \
)\ ]
;] ^
} 
} Ú
WD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\ITrainersService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{ 
internal		 
	interface		 
ITrainersService		 '
{

 
} 
} 
WD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\ITraineesService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{ 
public		 

	interface		 
ITraineesService		 %
{

 
} 
} ´
]D:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IStaticDropdownService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{		 
public

 

	interface

 "
IStaticDropdownService

 +
{ 
List 
< 
SelectListItem 
> 
GetTrainingTypes -
(- .
). /
;/ 0
List 
< 
SelectListItem 
> 
GetTrainingAnimal .
(. /
)/ 0
;0 1
} 
} ¥
SD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\ISiteService.cs
	namespace

 	
Apha


 
.

 
BST

 
.

 
Application

 
.

 

Interfaces

 )
{ 
public 

	interface 
ISiteService !
{ 
Task 
< 
IEnumerable 
< 
SiteDto  
>  !
>! "
GetAllSitesAsync# 3
(3 4
string4 :
plantNo; B
)B C
;C D
Task 
< 
List 
< 
SiteTraineeDto  
>  !
>! " 
GetSiteTraineesAsync# 7
(7 8
string8 >
plantNo? F
)F G
;G H
Task 
< 
string 
> 
DeleteTraineeAsync '
(' (
int( +
personId, 4
)4 5
;5 6
Task 
< 
string 
> 
AddSiteAsync !
(! "
SiteDto" )
siteDto* 1
,1 2
string3 9
userName: B
)B C
;C D
Task 
< 
string 
> 
UpdateSiteAsync $
($ %
SiteInputDto% 1
siteInputDto2 >
)> ?
;? @
} 
} Œ
ZD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IRoleMappingService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{		 
public

 

	interface

 
IRoleMappingService

 (
{ 
public 
Task 
< 
string 
> 
GetRoleName (
(( )
byte) -
roleId. 4
)4 5
;5 6
List 
< 
SelectListItem 
> 
GetUserLevels *
(* +
)+ ,
;, -
} 
} è
UD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IReportService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{		 
public

 

	interface

 
IReportService

 #
{ 
Task 
< 
List 
< 
SiteReportDto 
>  
>  !
GetSiteReportsAsync" 5
(5 6
)6 7
;7 8
Task 
< 
List 
< 
TrainerReportDto "
>" #
># $"
GetTrainerReportsAsync% ;
(; <
)< =
;= >
Task 
< 
List 
< 
PeopleReportDto !
>! "
>" #!
GetPeopleReportsAsync$ 9
(9 :
): ;
;; <
Task 
< 
List 
< 
TrainingReportDto #
># $
>$ %#
GetTrainingReportsAsync& =
(= >
)> ?
;? @
Task 
< 
List 
< 
AphaReportDto 
>  
>  !
GetAphaReportsAsync" 5
(5 6
)6 7
;7 8
Task 
< 
( 
byte 
[ 
] 
FileContents !
,! "
string# )
FileName* 2
)2 3
>3 4$
GenerateExcelReportAsync5 M
(M N
)N O
;O P
} 
} ª
VD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IPersonsService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{		 
public

 

	interface

 
IPersonsService

 $
{ 
Task 
< 
IEnumerable 
< 
PersonDetailDto (
>( )
>) *#
GetAllPersonByNameAsync+ B
(B C
intC F
personIdG O
)O P
;P Q
Task 
< 
IEnumerable 
< 
PersonLookupDto (
>( )
>) *&
GetPersonsForDropdownAsync+ E
(E F
)F G
;G H
Task 
< 
string 
> 
DeletePersonAsync &
(& '
int' *
personId+ 3
)3 4
;4 5
Task 
< 
IEnumerable 
< 
PersonSiteLookupDto ,
>, -
>- .
GetAllSitesAsync/ ?
(? @
string@ F
plantNoG N
)N O
;O P
Task 
< 
string 
> 
AddPersonAsync #
(# $
AddPersonDto$ 0

personsDto1 ;
,; <
string= C
userNameD L
)L M
;M N
Task 
< 
string 
> 
UpdatePersonAsync &
(& '
EditPersonDto' 4
dto5 8
)8 9
;9 :
Task 
< 
string 
? 
> "
GetPersonNameByIdAsync ,
(, -
int- 0
personId1 9
)9 :
;: ;
Task 
< 
string 
? 
> 
GetSiteByIdAsync &
(& '
int' *
personId+ 3
)3 4
;4 5
} 
} ”
SD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\INewsService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{		 
public

 

	interface

 
INewsService

 !
{ 
Task 
< 
IEnumerable 
< 
NewsDto  
>  !
>! "
GetLatestNewsAsync# 5
(5 6
)6 7
;7 8
Task 
< 
string 
> 
AddNewsAsync !
(! "
NewsDto" )
dto* -
)- .
;. /
Task 
< 
List 
< 
NewsDto 
> 
> 
GetNewsAsync (
(( )
)) *
;* +
Task 
< 
string 
> 
DeleteNewsAsync $
($ %
string% +
title, 1
)1 2
;2 3
} 
} ¢
XD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IDataEntryService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{ 
public		 

	interface		 
IDataEntryService		 &
{

 
Task 
< 
bool 
> 
CanEditPage 
( 
string %
action& ,
), -
;- .
} 
} á
WD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IAuditLogService.cs
	namespace

 	
Apha


 
.

 
BST

 
.

 
Application

 
.

 

Interfaces

 )
{ 
public 

	interface 
IAuditLogService %
{ 
Task 
< 
PaginatedResult 
< 
AuditLogDto (
>( )
>) *
GetAuditLogsAsync+ <
(< =
QueryParameters= L
filterM S
,S T
stringU [
storedProcedure\ k
)k l
;l m
Task 
< 
List 
< 
string 
> 
> (
GetStoredProcedureNamesAsync 7
(7 8
)8 9
;9 :
Task  
ArchiveAuditLogAsync !
(! "
string" (
userName) 1
)1 2
;2 3
} 
} õ
_D:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IAuditlogArchivedService.cs
	namespace		 	
Apha		
 
.		 
BST		 
.		 
Application		 
.		 

Interfaces		 )
{

 
public 

	interface $
IAuditlogArchivedService -
{ 
Task 
< 
PaginatedResult 
< 
AuditLogArchivedDto 0
>0 1
>1 2$
GetArchiveAuditLogsAsync3 K
(K L
QueryParametersL [
filter\ b
,b c
stringd j
storedProcedurek z
)z {
;{ |
Task 
< 
List 
< 
string 
> 
> (
GetStoredProcedureNamesAsync 7
(7 8
)8 9
;9 :
} 
} ÷
\D:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\Interfaces\IAccessControlService.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 

Interfaces )
{ 
public		 

	interface		 !
IAccessControlService		 *
{

 
Task 
< 
( 
byte 
? 
RoleId 
, 
string "
?" #
Username$ ,
), -
?- .
>. /,
 GetRoleIdAndUsernameByEmailAsync0 P
(P Q
stringQ W
emailX ]
)] ^
;^ _
} 
} Ä

JD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\VlaLocDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public

 

class

 
	VlaLocDto

 
{ 
public 
string 
LocId 
{ 
get !
;! "
set# &
;& '
}( )
=* +
null, 0
!0 1
;1 2
public 
string 
? 
VlaLocation "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
string 
? 
Ahvla 
{ 
get "
;" #
set$ '
;' (
}) *
public 
virtual 
ICollection "
<" #
Trainers# +
>+ ,
TblTrainers- 8
{9 :
get; >
;> ?
set@ C
;C D
}E F
=G H
newI L
ListM Q
<Q R
TrainersR Z
>Z [
([ \
)\ ]
;] ^
} 
} å
LD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\UserViewDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
UserViewDto		 
{

 
public 
string 
UserId 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
null- 1
!1 2
;2 3
public 
string 
? 
UserName 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
UserLoc 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
VlaLocation "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
byte 
? 
	UserLevel 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
UserLevelName $
{% &
get' *
;* +
set, /
;/ 0
}1 2
} 
} Æ

HD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\UserDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public

 

class

 
UserDto

 
{ 
public 
string 
UserId 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
null- 1
!1 2
;2 3
public 
string 
? 
UserName 
{  !
get" %
;% &
set' *
;* +
}, -
[ 	
Display	 
( 
Name 
= 
$str &
)& '
]' (
public 
string 
? 
UserLoc 
{  
get! $
;$ %
set& )
;) *
}+ ,
[ 	
Display	 
( 
Name 
= 
$str $
)$ %
]% &
public 
byte 
? 
	UserLevel 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} π	
RD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\TrainingReportDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
TrainingReportDto		 "
{

 
public 
string 
? 
Trainer 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
Trainee 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
	TrainedOn  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
? 
TrainingAnimal %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 
string 
? 
VLA 
{ 
get  
;  !
set" %
;% &
}' (
} 
} Û
LD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\TrainingDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public

 

class

 
TrainingDto

 
{ 
[ 	
Required	 
] 
public 
int 
PersonId 
{ 
get !
;! "
set# &
;& '
}( )
[ 	
Required	 
] 
public 
string 
TrainingAnimal $
{% &
get' *
;* +
set, /
;/ 0
}1 2
=3 4
null5 9
!9 :
;: ;
[ 	
Required	 
] 
public 
DateTime 
TrainingDateTime (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
[ 	
Required	 
] 
public 
string 
TrainingType "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
null3 7
!7 8
;8 9
[ 	
Required	 
] 
public 
int 
	TrainerId 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} Ì
SD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\TrainerTrainingDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
TrainerTrainingDto		 #
{

 
public 
int 
PersonID 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Person 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
TrainingAnimal %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 
string 
? 
TrainingType #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
public 
DateTime 
TrainingDateTime (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 
int 
	TraineeId 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} ü	
RD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\TrainerTrainedDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
TrainerTrainedDto		 "
{

 
public 
int 
	TraineeNo 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
? 
Trainee 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
Site 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
SpeciesTrained %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 
DateTime 
DateTrained #
{$ %
get& )
;) *
set+ .
;. /
}0 1
} 
} Ù
QD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\TrainerReportDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
TrainerReportDto		 !
{

 
public 
string 
? 
ID 
{ 
get 
;  
set! $
;$ %
}& '
public 
string 
? 
Trainer 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
int 
Trained 
{ 
get  
;  !
set" %
;% &
}' (
public 
int 
RunTot 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
Excel 
{ 
get 
; 
set  #
;# $
}% &
} 
} Ê
RD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\TrainerHistoryDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
TrainerHistoryDto		 "
{

 
public 
int 
PersonID 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Person 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
Role 
{ 
get !
;! "
set# &
;& '
}( )
public 
int 
	TrainerID 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
? 
TrainingAnimal %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 
DateTime 
TrainingDateTime (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 
string 
? 
Trainer 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} …
KD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\TraineeDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
[

 
Keyless

 
]

 
public 

class 

TraineeDto 
{ 
public 
int 
PersonId 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Person 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} —
OD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\SiteTraineeDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
SiteTraineeDto		 
{

 
public 
int 
PersonId 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Person 
{ 
get  #
;# $
set% (
;( )
}* +
public 
bool 
HasTraining 
{  !
get" %
;% &
internal' /
set0 3
;3 4
}5 6
} 
} Ü
ND:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\SiteReportDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
SiteReportDto		 
{

 
public 
string 
? 
PlantNo 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Add1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Add2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Town 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
County 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
Postcode 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
Phone 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
? 
Fax 
{ 
get  
;  !
set" %
;% &
}' (
public 
int 
People 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
RunTot 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
Excel 
{ 
get 
; 
set  #
;# $
}% &
} 
} ”
MD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\SiteInputDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public

 

class

 
SiteInputDto

 
{ 
[ 	
Required	 
( 
ErrorMessage 
=  
$str! ?
)? @
]@ A
public 
string 
PlantNo 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
null. 2
!2 3
;3 4
[ 	
Required	 
( 
ErrorMessage 
=  
$str! 9
)9 :
]: ;
public 
required 
string 
Name #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
? 
AddressLine1 #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
? 
AddressLine2 #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
? 
AddressTown "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
string 
? 
AddressCounty $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 
string 
? 
AddressPostCode &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
string 
? 
	Telephone  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
? 
Fax 
{ 
get  
;  !
set" %
;% &
}' (
public   
bool   
IsAhvla   
{   
get   !
;  ! "
set  # &
;  & '
}  ( )
}!! 
}"" ÿ
HD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\SiteDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public

 

class

 
SiteDto

 
{ 
[ 	
Required	 
( 
ErrorMessage 
=  
$str! ?
)? @
]@ A
public 
string 
PlantNo 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
null. 2
!2 3
;3 4
[ 	
Required	 
( 
ErrorMessage 
=  
$str! 9
)9 :
]: ;
public 
required 
string 
Name #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
? 
AddressLine1 #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
? 
AddressLine2 #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
? 
AddressTown "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
string 
? 
AddressCounty $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 
string 
? 
AddressPostCode &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
string 
? 
	Telephone  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
? 
Fax 
{ 
get  
;  !
set" %
;% &
}' (
public   
string   
?   
Ahvla   
{   
get   "
;  " #
set  $ '
;  ' (
}  ) *
}!! 
}"" È
JD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\ReportDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
	ReportDto		 
{

 
public 
int 
ReportId 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 

ReportName !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
GeneratedDate %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 
string 
? 
GeneratedBy "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
} Á
TD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\PersonSiteLookupDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public 

class 
PersonSiteLookupDto $
{ 
public 
string 
PlantNo 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
null. 2
!2 3
;3 4
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
} 
} 
KD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\PersonsDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 

PersonsDto		 
{

 
public 
int 
PersonId 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Person 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 

LocationId !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
int 
? 
VlalocationId !
{" #
get$ '
;' (
set) ,
;, -
}. /
} 
} †
PD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\PersonLookupDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public

 

class

 
PersonLookupDto

  
{ 
public 
int 
PersonID 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Person 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} Ò
PD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\PersonDetailDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public

 

class

 
PersonDetailDto

  
{ 
public 
int 
PersonID 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string	 
? 
Person 
{ 
get 
; 
set  #
;# $
}% &
public 
string 
? 
PlantNo 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
} 
} å
PD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\PeopleReportDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
PeopleReportDto		  
{

 
public 
string 
? 
PersonId 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
Person 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 

LocationId !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
string 
? 
AphaLocation #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
? 
Trainer 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
Trainee 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
Trained 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} 
HD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\NewsDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public

 

class

 
NewsDto

 
{ 
public 
string 
? 
Title 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
? 
NewsContent "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
DateTime 
DatePublished %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 
string 
? 
Author 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} ƒ
PD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\EditTrainingDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
EditTrainingDto		  
{

 
public 
int 
	TraineeId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
int 
	TrainerId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
required 
string 
TrainingType +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
public 
required 
string 
TrainingAnimal -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
public 
DateTime 
TrainingDateTime (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 
int 
TraineeIdOld 
{  !
get" %
;% &
set' *
;* +
}, -
public 
int 
TrainerIdOld 
{  !
get" %
;% &
set' *
;* +
}, -
public 
required 
string 
TrainingAnimalOld 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
public 
DateTime 
TrainingDateTimeOld +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
} 
} ±	
LD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\AuditLogDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
AuditLogDto		 
{

 
public 
string 
? 
	Procedure  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
? 

Parameters !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
string 
? 
User 
{ 
get !
;! "
set# &
;& '
}( )
public 
DateTime 
? 
Date 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
TransactionType &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
} 
} Û
ND:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\EditPersonDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public

 

class

 
EditPersonDto

 
{ 
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
public 
int 
PersonID 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Person 
{ 
get "
;" #
set$ '
;' (
}) *
=* +
null+ /
!/ 0
;0 1
} 
} ¡	
TD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\AuditLogArchivedDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
AuditLogArchivedDto		 $
{

 
public 
string 
? 
	Procedure  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
? 

Parameters !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
string 
? 
User 
{ 
get !
;! "
set# &
;& '
}( )
public 
DateTime 
? 
Date 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
TransactionType &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
} 
} —
ND:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\AphaReportDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{ 
public		 

class		 
AphaReportDto		 
{

 
public 
string 
? 
ID 
{ 
get 
;  
set! $
;$ %
}& '
public 
string 
? 
Location 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
APHA 
{ 
get !
;! "
set# &
;& '
}( )
} 
} Ü
MD:\a\apha-bst\apha-bst\src\Apha.BST\Apha.BST.Application\DTOs\AddPersonDto.cs
	namespace 	
Apha
 
. 
BST 
. 
Application 
. 
DTOs #
{		 
public

 

class

 
AddPersonDto

 
{ 
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 

LocationId !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
string 
PlantNo 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
null. 2
!2 3
;3 4
} 
} 