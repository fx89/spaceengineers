using IngameScript.drawing_framework;
using IngameScript.drawing_framework.sprites;
using IngameScript.terminal_utils;
using IngameScript.ui_framework;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;
using MySprite = IngameScript.drawing_framework.MySprite;

namespace IngameScript { partial class Program : MyGridProgram {

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




// README //////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CONFIGURATION ///////////////////////////////////////////////////////////////////////////////////////////////////

// Name of the 1x1 LCD panel to which the view should draw
private const string TARGET_LCD_PANEL_NAME = "3D_RENDERING_SCREEN";

// false = white foreground on black background
// true  = black foreground on white background
private static bool INVERT_COLORS = false;

// For how many frames should the script display the "INITIALIZING" message on screen
private const int POST_SCREEN_DURATION = 10;

// Distance of the rendered object from the view
//    --- increase this if you get the "Script too Complex" error while rendering
private const double MODEL_DISTANCE_FROM_VIEW = 7;

// Rotation angles in radians (how much should the object turn each frame)
private const double ROT_SPEED_RAD_YAW   = 0.10d;
private const double ROT_SPEED_RAD_PITCH = 0.05d;
private const double ROT_SPEED_RAD_ROLL  = 0.02d;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// WAVEFRONT OBJ LOADING ///////////////////////////////////////////////////////////////////////////////////////////

// This is a simple format which allows defining simple geometry, such as vertices and faces, in text mode.
// This format is available for export in almost every 3D graphics software and it's also easy to understand
// and parse. For more information on the Wavefront OBJ format, please visit this link:
//          https://en.wikipedia.org/wiki/Wavefront_.obj_file

private void InitSprites1() {

// IMPORTANT NOTE:
// ========================================================
// Make sure that none of the lines has leading spaces. The
// parser does not handle leading spaces to avoid running
// into the "Script too Complex" error.
//
ObjFileLines



// INSERT OBJ FILE HERE /////////////////////////////////////////////
= @"
v 0.981880 2.549078 -3.896440
v 0.981880 1.421911 -3.896440
v 1.026716 2.591203 -1.997988
v 1.026716 1.379280 -1.997988
v -0.981880 2.549078 -3.896440
v -0.981880 1.421911 -3.896440
v -1.026716 2.591203 -1.997988
v -1.026716 1.379280 -1.997988
v -2.751089 1.641909 -3.435606
v -2.757608 1.638021 -2.543591
v 2.751089 2.329084 -3.435606
v 2.751089 1.641909 -3.435606
v -2.757608 2.332968 -2.543591
v 2.757608 2.332968 -2.543591
v 2.757608 1.638021 -2.543591
v -2.751089 2.329084 -3.435606
v -0.434030 1.500424 2.035851
v -0.434030 2.341604 2.035851
v 0.434030 2.341603 2.035851
v 0.434030 1.500424 2.035851
v 0.353672 1.567967 -5.257379
v -0.353672 1.567967 -5.257378
v 0.353672 2.403027 -5.257379
v -0.353672 2.403027 -5.257379
v 0.202105 1.699045 -5.655478
v -0.202105 1.699045 -5.655477
v 0.202105 2.271948 -5.655478
v -0.202105 2.271948 -5.655478
v 0.119221 1.766184 -5.582514
v -0.119221 1.766184 -5.582514
v 0.119221 2.204809 -5.582514
v -0.119221 2.204809 -5.582514
v -0.281341 1.583701 3.505857
v -0.281341 2.077492 3.505857
v 0.281341 2.077492 3.505857
v 0.281341 1.583701 3.505857
v -3.559242 1.814946 -3.263240
v -3.559614 1.814738 -2.721392
v 3.559242 2.156047 -3.263240
v 3.559242 1.814946 -3.263240
v -3.559614 2.156256 -2.721392
v 3.559614 2.156256 -2.721392
v 3.559614 1.814738 -2.721392
v -3.559242 2.156047 -3.263240
v 0.523473 1.407853 0.006651
v -0.523473 1.407853 0.006651
v -0.523473 2.546668 0.006651
v 0.523473 2.546668 0.006651
v 1.441818 1.985497 -4.343869
v -1.549798 1.985178 -1.533668
v 1.549798 1.985178 -1.533668
v -1.441818 1.985497 -4.343869
v 2.829087 1.985497 -3.796525
v -2.842432 1.985493 -2.181122
v 2.842432 1.985493 -2.181122
v -2.829087 1.985497 -3.796525
v -0.809928 1.919904 2.049667
v 0.809928 1.919904 2.049667
v 0.646718 1.985497 -5.294322
v -0.646718 1.985497 -5.294322
v 0.376444 1.985497 -5.655268
v -0.376444 1.985497 -5.655268
v 0.165412 1.985497 -5.564027
v -0.165412 1.985497 -5.564028
v -0.390604 1.817469 3.665947
v 0.390604 1.817469 3.665947
v 3.657147 1.985497 -3.365551
v -3.657679 1.985497 -2.619078
v 3.657679 1.985497 -2.619078
v -3.657147 1.985497 -3.365551
v -0.969508 1.977217 0.044808
v 0.969508 1.977217 0.044808
v -1.001350 1.321006 -2.970192
v -0.000000 1.320628 -3.883773
v 1.344766 1.684775 -4.250362
v -1.437761 2.315701 -1.629842
v 1.437761 1.654666 -1.629842
v -1.344766 1.684775 -4.250362
v -0.000000 2.704554 -1.995754
v 1.001350 2.649928 -2.970192
v 0.000000 1.265828 -1.995754
v -1.001350 2.649928 -2.970192
v 0.000000 2.650359 -3.883772
v 1.001350 1.321006 -2.970192
v -2.735184 1.587581 -2.990388
v 2.803825 1.788739 -3.709919
v -2.815437 2.185580 -2.268074
v 2.815437 1.785407 -2.268074
v -2.803825 1.788739 -3.709919
v 2.735184 2.383412 -2.990388
v -2.735185 2.383412 -2.990388
v 2.735184 1.587581 -2.990388
v 1.994067 1.510283 -2.386629
v 1.994067 2.460651 -2.386629
v 1.973973 2.446783 -3.573617
v 1.973973 1.524210 -3.573617
v -1.994067 1.510283 -2.386629
v -1.973973 1.524210 -3.573617
v -1.973973 2.446783 -3.573617
v -1.994067 2.460651 -2.386629
v -0.716784 2.163241 2.045018
v 0.716784 1.677330 2.045018
v 0.000000 2.405115 2.036587
v -0.000000 1.436747 2.036587
v 0.472887 1.450771 1.016997
v -0.658853 1.381403 -0.977094
v -0.472887 2.466784 1.016997
v 0.658853 2.585829 -0.977094
v -0.000000 1.502372 -5.249417
v 0.575604 1.746073 -5.283457
v -0.575604 1.746073 -5.283457
v 0.000000 2.468622 -5.249417
v 0.550895 2.483734 -4.700709
v -0.550895 2.483734 -4.700709
v 0.550895 1.487259 -4.700709
v -0.550895 1.487259 -4.700709
v -0.000000 1.656101 -5.655001
v 0.333376 1.819488 -5.655397
v -0.333376 1.819488 -5.655397
v 0.000000 2.314892 -5.655001
v 0.256868 2.328049 -5.562745
v -0.256868 2.328049 -5.562745
v 0.256868 1.642944 -5.562745
v -0.256868 1.642944 -5.562745
v 0.000000 1.755807 -5.574754
v 0.152383 1.851162 -5.569134
v -0.152383 1.851162 -5.569134
v 0.000000 2.215187 -5.574754
v -0.160424 2.233692 -5.628733
v -0.160424 1.737301 -5.628733
v 0.160424 1.737301 -5.628733
v 0.160424 2.233692 -5.628733
v -0.359798 1.965769 3.624577
v 0.359798 1.676250 3.624577
v 0.000000 2.079427 3.579424
v -0.000000 1.570385 3.579424
v -0.370997 2.191895 2.946433
v 0.370997 2.191895 2.946433
v -0.370997 1.547363 2.946433
v 0.370997 1.547363 2.946433
v -3.584213 1.810476 -2.992355
v 3.628271 1.886687 -3.337127
v -3.628751 2.084450 -2.647510
v 3.628751 1.886544 -2.647510
v -3.628271 1.886687 -3.337127
v 3.584213 2.160517 -2.992355
v -3.584213 2.160517 -2.992355
v 3.584213 1.810476 -2.992355
v -3.290974 1.746968 -2.633168
v -3.289396 1.747858 -3.350611
v 3.289396 1.747858 -3.350611
v 3.290975 1.746968 -2.633168
v 3.290974 2.224025 -2.633168
v 3.289396 2.223136 -3.350611
v -3.289396 2.223136 -3.350611
v -3.290974 2.224025 -2.633168
v 0.658853 1.381403 -0.977094
v -0.472887 1.450771 1.016997
v -0.658853 2.585830 -0.977094
v 0.472887 2.466784 1.016997
v -0.860034 2.304476 0.033556
v 0.860034 1.649989 0.033556
v -0.000000 2.635907 -0.001043
v 0.000000 1.318609 -0.001043
v 1.344766 2.286218 -4.250362
v -1.437761 1.654666 -1.629842
v 1.437761 2.315701 -1.629842
v -1.344766 2.286218 -4.250362
v 2.803825 2.182255 -3.709919
v -2.815437 1.785407 -2.268074
v 2.815437 2.185580 -2.268074
v -2.803825 2.182255 -3.709919
v -0.716784 1.677330 2.045018
v 0.716784 2.163241 2.045018
v 0.575604 2.224921 -5.283457
v -0.575604 2.224921 -5.283457
v 0.333376 2.151505 -5.655396
v -0.333376 2.151505 -5.655396
v 0.152383 2.119832 -5.569134
v -0.152383 2.119832 -5.569134
v -0.359798 1.676250 3.624577
v 0.359798 1.965769 3.624577
v 3.628271 2.084306 -3.337127
v -3.628750 1.886544 -2.647510
v 3.628751 2.084450 -2.647510
v -3.628271 2.084306 -3.337127
v -0.860034 1.649989 0.033556
v 0.860034 2.304476 0.033556
v 1.153193 1.983604 -0.835071
v -1.153193 1.983604 -0.835071
v -3.357937 1.985496 -2.361020
v -3.354839 1.985497 -3.622446
v 3.354839 1.985497 -3.622446
v 3.357937 1.985496 -2.361020
v 3.748945 1.985497 -2.992395
v -3.748945 1.985497 -2.992395
v 0.660755 1.865514 2.994153
v -0.660755 1.865514 2.994153
v 0.000000 1.799254 3.899440
v 0.285654 1.985497 -5.622963
v 0.000000 1.985497 -5.539076
v -0.479942 1.985497 -5.570518
v 0.479942 1.985497 -5.570518
v -0.285654 1.985497 -5.622963
v -0.943197 1.985497 -4.839129
v 0.943197 1.985497 -4.839129
v -0.888370 1.958533 1.027920
v 0.888370 1.958533 1.027920
v -2.181411 1.985457 -1.954546
v -2.138024 1.985497 -3.999693
v 2.181411 1.985457 -1.954546
v 2.138024 1.985497 -3.999693
v 2.091397 1.725908 -3.901435
v 2.128894 2.256239 -2.053998
v -2.091397 2.245086 -3.901435
v -2.128894 1.714678 -2.053998
v 0.785147 1.665680 1.024477
v -0.785147 2.251558 1.024477
v 0.852540 2.266602 -4.801911
v -0.852540 1.704391 -4.801911
v -0.255074 1.839609 -5.624842
v 0.424840 2.183027 -5.568175
v -0.424840 1.787966 -5.568175
v 0.000000 1.851044 -5.547434
v 0.255074 2.131384 -5.624842
v -0.000000 1.668928 3.830159
v -0.589867 2.053809 2.979086
v 0.589867 1.679872 2.979086
v -3.709515 1.894507 -2.992393
v 3.709515 1.894507 -2.992393
v 3.335553 2.123507 -2.426389
v 3.332834 1.848264 -3.557151
v -3.332834 2.122730 -3.557151
v -3.335553 1.847486 -2.426389
v -1.036905 2.325744 -0.873176
v 1.036905 1.641470 -0.873176
v 0.000000 1.282883 -0.996404
v -0.000000 2.684340 -0.996404
v 1.036905 2.325744 -0.873176
v -1.036905 1.641470 -0.873176
v -3.335553 2.123507 -2.426389
v -3.332834 1.848264 -3.557151
v -3.280776 2.255795 -2.992025
v 3.332834 2.122730 -3.557151
v 3.335553 1.847486 -2.426389
v 3.280776 2.255795 -2.992025
v 3.709515 2.076486 -2.992393
v -3.709515 2.076486 -2.992393
v 0.589867 2.053809 2.979086
v -0.000000 1.503367 2.956560
v -0.589867 1.679872 2.979086
v 0.000000 1.941235 3.830159
v 0.255074 1.839609 -5.624842
v 0.000000 2.266416 -5.627577
v 0.000000 1.704578 -5.627577
v 0.000000 2.119949 -5.547434
v -0.000000 1.590100 -5.560753
v -0.424840 2.183027 -5.568175
v 0.424840 1.787966 -5.568175
v -0.255074 2.131384 -5.624842
v 0.000000 1.405829 -4.679379
v -0.852540 2.266602 -4.801911
v 0.852540 1.704391 -4.801911
v 0.000000 2.380893 -5.560753
v -0.785147 1.665680 1.024477
v 0.785147 2.251558 1.024477
v -0.000000 2.545212 1.015147
v 0.000000 2.234303 2.956560
v -2.128894 2.256239 -2.053998
v -2.091397 1.725908 -3.901435
v 1.955631 2.524752 -2.984460
v -1.955631 2.524752 -2.984460
v -1.955631 1.446232 -2.984460
v 1.955631 1.446232 -2.984460
v -3.280776 1.715199 -2.992025
v 3.280776 1.715199 -2.992025
v 0.000000 2.565164 -4.679379
v 2.128894 1.714678 -2.053998
v 0.000000 1.247991 -2.965647
v 2.091397 2.245086 -3.901435
v 0.000000 1.372328 1.015147
v 0.000000 2.722929 -2.965647
f 282/1/1 82/2/1 7/3/1 79/4/1
f 281/5/2 105/6/2 20/7/2 104/8/2
f 280/9/3 95/10/3 11/11/3 169/12/3
f 279/13/4 84/14/4 4/15/4 81/16/4
f 278/17/5 93/18/5 15/19/5 88/20/5
f 277/21/6 113/22/6 23/23/6 112/24/6
f 276/25/7 151/26/7 40/27/7 148/28/7
f 275/29/8 149/30/8 38/31/8 141/32/8
f 274/33/9 96/34/9 12/35/9 92/36/9
f 273/37/10 97/38/10 10/39/10 85/40/10
f 272/41/11 99/42/11 16/43/11 91/44/11
f 271/45/12 94/46/12 14/47/12 90/48/12
f 270/49/13 98/50/13 9/51/13 89/52/13
f 269/53/14 100/54/14 13/55/14 87/56/14
f 268/57/15 137/58/15 34/59/15 135/60/15
f 267/61/16 107/62/16 18/63/16 103/64/16
f 266/65/17 160/66/17 19/67/17 174/68/17
f 265/69/18 158/70/18 17/71/18 173/72/18
f 264/73/19 121/74/19 27/75/19 120/76/19
f 263/77/20 115/78/20 21/79/20 110/80/20
f 262/81/21 114/82/21 24/83/21 176/84/21
f 261/85/22 116/86/22 22/87/22 109/88/22
f 260/89/23 129/90/23 32/91/23 180/92/23
f 259/93/24 123/94/24 25/95/24 118/96/24
f 258/97/25 122/98/25 28/99/25 178/100/25
f 257/101/26 124/102/26 26/103/26 117/104/26
f 256/105/27 128/106/27 31/107/27 179/108/27
f 255/109/28 130/110/28 30/111/28 125/112/28
f 254/113/29 132/114/29 31/107/29 128/115/29
f 253/116/30 131/117/30 29/118/30 126/119/30
f 252/120/31 135/121/31 34/122/31 133/123/31
f 251/124/32 139/125/32 33/126/32 181/127/32
f 250/128/33 140/129/33 36/130/33 136/131/33
f 249/132/34 138/133/34 35/134/34 182/135/34
f 248/136/35 147/137/35 44/138/35 186/139/35
f 247/140/36 146/141/36 42/142/36 185/143/36
f 246/144/37 153/145/37 42/142/37 146/141/37
f 245/146/38 152/147/38 43/148/38 144/149/38
f 244/150/39 154/151/39 39/152/39 183/153/39
f 243/154/40 155/155/40 44/156/40 147/157/40
f 242/158/41 150/159/41 37/160/41 145/161/41
f 241/162/42 156/163/42 41/164/42 143/165/42
f 240/166/43 106/167/43 46/168/43 187/169/43
f 239/170/44 108/171/44 48/172/44 188/173/44
f 238/174/45 159/175/45 47/176/45 163/177/45
f 237/178/46 157/179/46 45/180/46 164/181/46
f 236/182/47 189/183/47 72/184/47 162/185/47
f 235/186/48 190/187/48 71/188/48 161/189/48
f 234/190/49 191/191/49 68/192/49 184/193/49
f 233/194/50 192/195/50 70/196/50 186/139/50
f 232/197/51 193/198/51 67/199/51 142/200/51
f 231/201/52 194/202/52 69/203/52 185/143/52
f 230/204/53 195/205/53 69/203/53 144/149/53
f 229/206/54 196/207/54 70/196/54 145/161/54
f 228/208/55 197/209/55 66/210/55 134/211/55
f 227/212/56 198/213/56 65/214/56 133/123/56
f 226/215/57 199/216/57 65/214/57 181/127/57
f 225/217/58 200/218/58 63/219/58 179/108/58
f 224/220/59 201/221/59 63/219/59 126/119/59
f 223/222/60 202/223/60 62/224/60 119/225/60
f 222/226/61 203/227/61 61/228/61 177/229/61
f 221/230/62 204/231/62 64/232/62 127/233/62
f 220/234/63 205/235/63 60/236/63 111/237/63
f 219/238/64 206/239/64 59/240/64 175/241/64
f 218/242/65 207/243/65 57/244/65 101/245/65
f 217/246/66 208/247/66 58/248/66 102/249/66
f 216/250/67 209/251/67 54/252/67 170/253/67
f 215/254/68 210/255/68 56/256/68 172/257/68
f 214/258/69 211/259/69 55/260/69 171/261/69
f 213/262/70 212/263/70 53/264/70 86/265/70
f 96/34/71 213/262/71 86/265/71 12/35/71
f 2/266/72 75/267/72 213/262/72 96/34/72
f 75/267/73 49/268/73 212/263/73 213/262/73
f 94/46/74 214/258/74 171/261/74 14/47/74
f 3/269/75 167/270/75 214/258/75 94/46/75
f 167/270/76 51/271/76 211/259/76 214/258/76
f 99/272/77 215/254/77 172/257/77 16/273/77
f 5/274/78 168/275/78 215/254/78 99/272/78
f 168/275/79 52/276/79 210/255/79 215/254/79
f 97/277/80 216/250/80 170/253/80 10/278/80
f 8/279/81 166/280/81 216/250/81 97/277/81
f 166/280/82 50/281/82 209/251/82 216/250/82
f 105/6/83 217/246/83 102/249/83 20/7/83
f 45/180/84 162/185/84 217/246/84 105/6/84
f 162/185/85 72/184/85 208/247/85 217/246/85
f 107/282/86 218/242/86 101/245/86 18/283/86
f 47/284/87 161/189/87 218/242/87 107/282/87
f 161/189/88 71/188/88 207/243/88 218/242/88
f 113/22/89 219/238/89 175/241/89 23/23/89
f 1/285/90 165/286/90 219/238/90 113/22/90
f 165/286/91 49/268/91 206/239/91 219/238/91
f 116/287/92 220/234/92 111/237/92 22/288/92
f 6/289/93 78/290/93 220/234/93 116/287/93
f 78/290/94 52/276/94 205/235/94 220/234/94
f 130/291/95 221/230/95 127/233/95 30/292/95
f 26/293/96 119/225/96 221/230/96 130/291/96
f 119/225/97 62/224/97 204/231/97 221/230/97
f 121/74/98 222/226/98 177/229/98 27/75/98
f 23/23/99 175/241/99 222/226/99 121/74/99
f 175/241/100 59/240/100 203/227/100 222/226/100
f 124/294/101 223/222/101 119/225/101 26/293/101
f 22/288/102 111/237/102 223/222/102 124/294/102
f 111/237/103 60/236/103 202/223/103 223/222/103
f 125/295/104 224/220/104 126/119/104 29/118/104
f 30/292/105 127/233/105 224/220/105 125/295/105
f 127/233/106 64/232/106 201/221/106 224/220/106
f 132/114/107 225/217/107 179/108/107 31/107/107
f 27/75/108 177/229/108 225/217/108 132/114/108
f 177/229/109 61/228/109 200/218/109 225/217/109
f 136/296/110 226/215/110 181/127/110 33/126/110
f 36/130/111 134/211/111 226/215/111 136/296/111
f 134/211/112 66/210/112 199/216/112 226/215/112
f 137/297/113 227/212/113 133/123/113 34/122/113
f 18/283/114 101/245/114 227/212/114 137/297/114
f 101/245/115 57/244/115 198/213/115 227/212/115
f 140/129/116 228/208/116 134/211/116 36/130/116
f 20/7/117 102/249/117 228/208/117 140/129/117
f 102/249/118 58/248/118 197/209/118 228/208/118
f 141/298/119 229/206/119 145/161/119 37/160/119
f 38/299/120 184/300/120 229/206/120 141/298/120
f 184/300/121 68/301/121 196/207/121 229/206/121
f 148/28/122 230/204/122 144/149/122 43/148/122
f 40/27/123 142/200/123 230/204/123 148/28/123
f 142/200/124 67/199/124 195/205/124 230/204/124
f 153/145/125 231/201/125 185/143/125 42/142/125
f 14/47/126 171/261/126 231/201/126 153/145/126
f 171/261/127 55/260/127 194/202/127 231/201/127
f 151/26/128 232/197/128 142/200/128 40/27/128
f 12/35/129 86/265/129 232/197/129 151/26/129
f 86/265/130 53/264/130 193/198/130 232/197/130
f 155/302/131 233/194/131 186/139/131 44/138/131
f 16/273/132 172/257/132 233/194/132 155/302/132
f 172/257/133 56/256/133 192/195/133 233/194/133
f 149/303/134 234/190/134 184/193/134 38/304/134
f 10/278/135 170/253/135 234/190/135 149/303/135
f 170/253/136 54/252/136 191/191/136 234/190/136
f 159/305/137 235/186/137 161/189/137 47/284/137
f 7/306/138 76/307/138 235/186/138 159/305/138
f 76/307/139 50/281/139 190/187/139 235/186/139
f 157/179/140 236/182/140 162/185/140 45/180/140
f 4/15/141 77/308/141 236/182/141 157/179/141
f 77/308/142 51/271/142 189/183/142 236/182/142
f 106/309/143 237/178/143 164/181/143 46/310/143
f 8/311/144 81/16/144 237/178/144 106/309/144
f 81/16/145 4/15/145 157/179/145 237/178/145
f 108/171/146 238/174/146 163/177/146 48/172/146
f 3/269/147 79/4/147 238/174/147 108/171/147
f 79/4/148 7/3/148 159/175/148 238/174/148
f 189/183/149 239/170/149 188/173/149 72/184/149
f 51/271/150 167/270/150 239/170/150 189/183/150
f 167/270/151 3/269/151 108/171/151 239/170/151
f 190/187/152 240/166/152 187/169/152 71/188/152
f 50/281/153 166/280/153 240/166/153 190/187/153
f 166/280/154 8/279/154 106/167/154 240/166/154
f 191/191/155 241/162/155 143/165/155 68/192/155
f 54/252/156 87/56/156 241/162/156 191/191/156
f 87/56/157 13/55/157 156/163/157 241/162/157
f 192/195/158 242/158/158 145/161/158 70/196/158
f 56/256/159 89/52/159 242/158/159 192/195/159
f 89/52/160 9/51/160 150/159/160 242/158/160
f 156/312/161 243/154/161 147/157/161 41/313/161
f 13/314/162 91/44/162 243/154/162 156/312/162
f 91/44/163 16/43/163 155/155/163 243/154/163
f 193/198/164 244/150/164 183/153/164 67/199/164
f 53/264/165 169/12/165 244/150/165 193/198/165
f 169/12/166 11/11/166 154/151/166 244/150/166
f 194/202/167 245/146/167 144/149/167 69/203/167
f 55/260/168 88/20/168 245/146/168 194/202/168
f 88/20/169 15/19/169 152/147/169 245/146/169
f 154/151/170 246/144/170 146/141/170 39/152/170
f 11/11/171 90/48/171 246/144/171 154/151/171
f 90/48/172 14/47/172 153/145/172 246/144/172
f 195/205/173 247/140/173 185/143/173 69/203/173
f 67/199/174 183/153/174 247/140/174 195/205/174
f 183/153/175 39/152/175 146/141/175 247/140/175
f 196/207/176 248/136/176 186/139/176 70/196/176
f 68/301/177 143/315/177 248/136/177 196/207/177
f 143/315/178 41/316/178 147/137/178 248/136/178
f 197/209/179 249/132/179 182/135/179 66/210/179
f 58/248/180 174/68/180 249/132/180 197/209/180
f 174/68/181 19/67/181 138/133/181 249/132/181
f 139/317/182 250/128/182 136/131/182 33/318/182
f 17/319/183 104/8/183 250/128/183 139/317/183
f 104/8/184 20/7/184 140/129/184 250/128/184
f 198/213/185 251/124/185 181/127/185 65/214/185
f 57/244/186 173/72/186 251/124/186 198/213/186
f 173/72/187 17/71/187 139/125/187 251/124/187
f 199/216/188 252/120/188 133/123/188 65/214/188
f 66/210/189 182/135/189 252/120/189 199/216/189
f 182/135/190 35/134/190 135/121/190 252/120/190
f 200/218/191 253/116/191 126/119/191 63/219/191
f 61/228/192 118/96/192 253/116/192 200/218/192
f 118/96/193 25/95/193 131/117/193 253/116/193
f 129/320/194 254/113/194 128/115/194 32/321/194
f 28/322/195 120/76/195 254/113/195 129/320/195
f 120/76/196 27/75/196 132/114/196 254/113/196
f 131/117/197 255/109/197 125/112/197 29/118/197
f 25/95/198 117/104/198 255/109/198 131/117/198
f 117/104/199 26/103/199 130/110/199 255/109/199
f 201/221/200 256/105/200 179/108/200 63/219/200
f 64/232/201 180/92/201 256/105/201 201/221/201
f 180/92/202 32/91/202 128/106/202 256/105/202
f 123/94/203 257/101/203 117/104/203 25/95/203
f 21/79/204 109/88/204 257/101/204 123/94/204
f 109/88/205 22/87/205 124/102/205 257/101/205
f 202/223/206 258/97/206 178/100/206 62/224/206
f 60/236/207 176/84/207 258/97/207 202/223/207
f 176/84/208 24/83/208 122/98/208 258/97/208
f 203/227/209 259/93/209 118/96/209 61/228/209
f 59/240/210 110/80/210 259/93/210 203/227/210
f 110/80/211 21/79/211 123/94/211 259/93/211
f 204/231/212 260/89/212 180/92/212 64/232/212
f 62/224/213 178/100/213 260/89/213 204/231/213
f 178/100/214 28/99/214 129/90/214 260/89/214
f 115/78/215 261/85/215 109/88/215 21/79/215
f 2/266/216 74/323/216 261/85/216 115/78/216
f 74/323/217 6/324/217 116/86/217 261/85/217
f 205/235/218 262/81/218 176/84/218 60/236/218
f 52/276/219 168/275/219 262/81/219 205/235/219
f 168/275/220 5/274/220 114/82/220 262/81/220
f 206/239/221 263/77/221 110/80/221 59/240/221
f 49/268/222 75/267/222 263/77/222 206/239/222
f 75/267/223 2/266/223 115/78/223 263/77/223
f 122/325/224 264/73/224 120/76/224 28/322/224
f 24/326/225 112/24/225 264/73/225 122/325/225
f 112/24/226 23/23/226 121/74/226 264/73/226
f 207/243/227 265/69/227 173/72/227 57/244/227
f 71/188/228 187/169/228 265/69/228 207/243/228
f 187/169/229 46/168/229 158/70/229 265/69/229
f 208/247/230 266/65/230 174/68/230 58/248/230
f 72/184/231 188/173/231 266/65/231 208/247/231
f 188/173/232 48/172/232 160/66/232 266/65/232
f 160/66/233 267/61/233 103/64/233 19/67/233
f 48/172/234 163/177/234 267/61/234 160/66/234
f 163/177/235 47/176/235 107/62/235 267/61/235
f 138/133/236 268/57/236 135/60/236 35/134/236
f 19/67/237 103/64/237 268/57/237 138/133/237
f 103/64/238 18/63/238 137/58/238 268/57/238
f 209/251/239 269/53/239 87/56/239 54/252/239
f 50/281/240 76/307/240 269/53/240 209/251/240
f 76/307/241 7/306/241 100/54/241 269/53/241
f 210/255/242 270/49/242 89/52/242 56/256/242
f 52/276/243 78/290/243 270/49/243 210/255/243
f 78/290/244 6/289/244 98/50/244 270/49/244
f 95/10/245 271/45/245 90/48/245 11/11/245
f 1/285/246 80/327/246 271/45/246 95/10/246
f 80/327/247 3/269/247 94/46/247 271/45/247
f 100/328/248 272/41/248 91/44/248 13/314/248
f 7/3/249 82/2/249 272/41/249 100/328/249
f 82/2/250 5/329/250 99/42/250 272/41/250
f 98/330/251 273/37/251 85/40/251 9/331/251
f 6/324/252 73/332/252 273/37/252 98/330/252
f 73/332/253 8/311/253 97/38/253 273/37/253
f 93/18/254 274/33/254 92/36/254 15/19/254
f 4/15/255 84/14/255 274/33/255 93/18/255
f 84/14/256 2/266/256 96/34/256 274/33/256
f 150/333/257 275/29/257 141/32/257 37/334/257
f 9/331/258 85/40/258 275/29/258 150/333/258
f 85/40/259 10/39/259 149/30/259 275/29/259
f 152/147/260 276/25/260 148/28/260 43/148/260
f 15/19/261 92/36/261 276/25/261 152/147/261
f 92/36/262 12/35/262 151/26/262 276/25/262
f 114/335/263 277/21/263 112/24/263 24/326/263
f 5/329/264 83/336/264 277/21/264 114/335/264
f 83/336/265 1/285/265 113/22/265 277/21/265
f 211/259/266 278/17/266 88/20/266 55/260/266
f 51/271/267 77/308/267 278/17/267 211/259/267
f 77/308/268 4/15/268 93/18/268 278/17/268
f 73/332/269 279/13/269 81/16/269 8/311/269
f 6/324/270 74/323/270 279/13/270 73/332/270
f 74/323/271 2/266/271 84/14/271 279/13/271
f 212/263/272 280/9/272 169/12/272 53/264/272
f 49/268/273 165/286/273 280/9/273 212/263/273
f 165/286/274 1/285/274 95/10/274 280/9/274
f 158/337/275 281/5/275 104/8/275 17/319/275
f 46/310/276 164/181/276 281/5/276 158/337/276
f 164/181/277 45/180/277 105/6/277 281/5/277
f 80/327/278 282/1/278 79/4/278 3/269/278
f 1/285/279 83/336/279 282/1/279 80/327/279
f 83/336/280 5/329/280 82/2/280 282/1/280
"
// END OF OBJ FILE //////////////////////////////////////////////////



.Split('\n');
}
private void InitSprites2() {
 // Make sure the model is not too complex for the script to handle
 // In effect, try to avoid the "Script too Complex" error as much as possible
    if (ObjFileLines.Length > 700) {
        throw new ArgumentException("The Wavefront OBJ you are trying to load has more than 700 lines. This makes it too complex for the script to handle.");
    }

 // Create the 3D object
    Obj3D = new MySimple3DObject();

 // Parse the first 231 lines of the Wavefront OBJ file into the 3D object
 // The loader will not do anything if the line index is outside the available range
    MySimpleWavefrontObjLoader.LoadFromArray(ObjFileLines, Obj3D, 0, 230);    
}
private void InitSprites3() {
 // Parse the next 230 lines of the Wavefront OBJ file into the 3D object
 // The loader will not do anything if the line index is outside the available range
    MySimpleWavefrontObjLoader.LoadFromArray(ObjFileLines, Obj3D, 231, 460);
}
private void InitSprites4() {
 // Parse the last 240 lines of the Wavefront OBJ file into the 3D object
 // The loader will not do anything if the line index is outside the available range
    MySimpleWavefrontObjLoader.LoadFromArray(ObjFileLines, Obj3D, 461, 700);

 // One may also create a simple model form code and attach it to the model view, like this:
 /*
    TheModelView.AttachModel(
        new MySimple3DObject()
              // Bottom plane
                .WithVertex(-1, -1, -1)
                .WithVertex(-1, -1,  1)
                .WithVertex( 1, -1,  1)
                .WithVertex( 1, -1, -1)
              // Top plane
                .WithVertex(-1,  1, -1)
                .WithVertex(-1,  1,  1)
                .WithVertex( 1,  1,  1)
                .WithVertex( 1,  1, -1)
              // Faces
                .WithFace(new int[]{0,1,2,3}) // bottom
                .WithFace(new int[]{4,5,6,7}) // top
                .WithFace(new int[]{0,4,7,3}) // front
                .WithFace(new int[]{1,5,6,2}) // back
                .WithFace(new int[]{0,4,5,1}) // left
                .WithFace(new int[]{3,7,6,2}) // right
    );
*/
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SMALL 3D FRAMEWORK //////////////////////////////////////////////////////////////////////////////////////////////

private class My3DModelView : MyOnScreenObject {

    private const int CENTER_X = RES_X / 2;
    private const int CENTER_Y = RES_Y / 2;

    private const double SCALE_X = 35d;
    private const double SCALE_Y = 20d;

    private MySimple3DObject AttachedModel;

    private int iterationNumber = 0;

    private MyScreen TargetScreen;

    private bool invertScreenColors = false;

    private double rotYaw   = 0.01d;
    private double rotPitch = 0.10d;
    private double rotRoll  = 0.03d;

    public My3DModelView(MyScreen TargetScreen, bool invertScreenColors) : base(null, 0, 0, true) {
        this.TargetScreen = TargetScreen;
        this.invertScreenColors = invertScreenColors;
    }

    public My3DModelView WithRotationSpeeds(double rotYaw, double rotPitch, double rotRoll) {
        this.rotYaw   = rotYaw;
        this.rotPitch = rotPitch;
        this.rotRoll  = rotRoll;

        return this;
    }

    public override int GetHeight() {
        return RES_Y;
    }

    public override int GetWidth() {
        return RES_X;
    }

    protected override void Compute(MyCanvas TargetCanvas) {
        
    }

    protected override void Draw(MyCanvas TargetCanvas) {
        if (AttachedModel != null) {
         // Splitting the operation across multiple iterations,
         // to avoid the "Script too Complex" error.
            switch (iterationNumber) {
             // The first iteration rotates the object
             // The canvas is also cleared at this point, but it is not yet
             // flushed to the target screen, to avoid flickering
                case 0:
                    AttachedModel.Rotate(rotYaw, rotPitch, rotRoll);
                    TargetCanvas.Clear();
                    break;

             // The second iteration draws the object
                case 1:
                    foreach (MyFace Face in AttachedModel.Faces) {
                        DrawFace(Face, TargetCanvas);
                    }
                    break;

             // The third operation flushes the buffer of the target canvas onto
             // the target screen
                case 2:
                    TargetScreen.FlushBufferToScreen(invertScreenColors);
                    break;

                default:
                    break;
            }

         // Go to the next iteration number
            iterationNumber++;
            if (iterationNumber == 3) {
                iterationNumber = 0;
            }
        }
    }

    private void DrawFace(MyFace Face, MyCanvas TargetCanvas) {
     // These will be the screen coordinates of the first vertex, which will be needed at the
     // end of the process, for closing the polygon
        MyPoint2D Origin = null;

     // This will hold the screen coordinates of the previous vertex, which will serve as the
     // origin point when drawing the edges of the polygon.
        MyPoint2D PrevScreenPos = null;

        foreach(MyPoint3D Vertex in Face.Vertices) {
         // If this is the first vertex, then there is no edge to draw yer.
         // However, the screen coordinates mist be computed and stored for the next iteration.
         // They will also be needed for closing the polygon at the end of the process.
            if(Origin == null) {
                Origin = WorldToScreenCoordinates(Vertex);
                PrevScreenPos = Origin;
            } else {
             // Get the screen coordinates of the vertex
                MyPoint2D ScreenPos = WorldToScreenCoordinates(Vertex);

             // Draw the line between the screen coordinates of the previous vertex and those of the current one
                TargetCanvas.DrawLine(
                    (int)PrevScreenPos.X, (int)PrevScreenPos.Y,
                    (int)ScreenPos.X, (int)ScreenPos.Y, true
                );

             // The screen coordinates of the previous vertex now become those of the current one
                PrevScreenPos = ScreenPos;
            }
        }

     // Finally, draw a line from the last vertex back to the first vertex, to close the polygon
        TargetCanvas.DrawLine(
            (int)PrevScreenPos.X, (int)PrevScreenPos.Y,
            (int)Origin.X, (int)Origin.Y, true
        );
    }

    /**
     * Appies a blunt transformation between 3D coordinates
     * and screen coordinates
     */
    private MyPoint2D WorldToScreenCoordinates(MyPoint3D Vertex) {
        MyPoint2D ret = new MyPoint2D(0, 0);

        double z = Vertex.Z + AttachedModel.Position.Z;
        if (z == 0) {
            z = 1;
        }

        ret.X = CENTER_X + (((Vertex.X + AttachedModel.Position.X) / z) * SCALE_X) + (z * 30 / SCALE_X);
        ret.Y = CENTER_Y + (((Vertex.Y + AttachedModel.Position.Y) / z) * SCALE_Y) + (z * 30 / SCALE_Y);

        return ret;
    }

    protected override void Init() {
        
    }

    public void AttachModel(MySimple3DObject Model) {
        AttachedModel = Model;
    }

    public void SetAttachedModelPosition(double x, double y, double z) {
        if (AttachedModel != null) {
            AttachedModel.Position.X = x;
            AttachedModel.Position.Y = y;
            AttachedModel.Position.Z = z;
        }
    }
}

/////////////////////////////////////////////////////

/**
 * Using custom type instead of the already available Vecor3D because that is
 * a struct and causes unwanted issues with the linkage between faces and vertices.
 */
private class MyPoint3D {
    public double X, Y, Z;

    public MyPoint3D(double x, double y, double z) {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}

/**
 * This will hold screen coordinates of vertices in the 3D environment
 */
private class MyPoint2D {
    public double X, Y;

    public MyPoint2D(double x, double y) {
        this.X = x;
        this.Y = y;
    }
}

/**
 * Stores references to vertices. This is why the Vector3D struct is not
 * usable. For this to work, we need to store references to the vertices,
 * so that when the vertices rotate, the faces will rotate automatically.
 */
private class MyFace {
    public List<MyPoint3D> Vertices = new List<MyPoint3D>();
}

/**
 * Stores simple geometry (vertices and faces)
 */
private class MySimple3DObject {
  /**
    * The object's position in 3D space
    */
    public Vector3D Position = new Vector3D(0, 0, 0);

 /**
   * List of vertices (mostly useful for loading faces)
   */
    public List<MyPoint3D> Vertices = new List<MyPoint3D>();

 /**
   * List of faces, which will be required for drawing
   */
    public List<MyFace> Faces = new List<MyFace>();

 /**
   * The corners of the object (might be required for re-centering)
   */
    public MyPoint3D NegativeCorner = new MyPoint3D(0,0,0);
    public MyPoint3D PositiveCorner = new MyPoint3D(0,0,0);

    public MySimple3DObject WithVertex(double x, double y, double z) {
     // Add the vertex
        Vertices.Add(new MyPoint3D(x, y, z));

     // Update the corner coordinates
        if (x < NegativeCorner.X) NegativeCorner.X = x;
        if (y < NegativeCorner.Y) NegativeCorner.Y = y;
        if (z < NegativeCorner.Z) NegativeCorner.Z = z;
        if (x > PositiveCorner.X) PositiveCorner.X = x;
        if (y > PositiveCorner.Y) PositiveCorner.Y = y;
        if (z > PositiveCorner.Z) PositiveCorner.Z = z;

     // Return a reference to the current object, so it can be use as a builder
        return this;
    }

    public MySimple3DObject WithFace(int[] vertIndexes) {
     // Validate the vertex indexes array
        if (vertIndexes == null || vertIndexes.Length < 3) {
            throw new ArgumentException("The face must have at least 3 vertices");
        }

     // Create a new face
        MyFace Face = new MyFace();

     // Link the vertices to the face
        foreach (int vIdx in vertIndexes) {
            MyPoint3D vertex = null;
            try {
                vertex = Vertices[vIdx];
            } catch (Exception exc) {
                throw new ArgumentException("Vertex number [" + vIdx + "] is not yet defined. " + exc.Message);
            }

            try {
                Face.Vertices.Add(vertex);
            } catch (Exception exc) {
                throw new ArgumentException("Could not add vertex to face: " + exc.Message);
            }
        }

     // Add the face to the list of faces
        Faces.Add(Face);

     // Return a reference to the current object, so it can be use as a builder
        return this;
    }

    public MySimple3DObject WithFace(List<int> vertIndexes) {
        return WithFace(vertIndexes.ToArray());
    }

  /**
    * Rotates all the vertices of the object using Vrage.Math, which
    * is provided by the Space Engineers API.
    */
    public void Rotate(double yaw, double pitch, double roll) {
     // Create the rotation matrix
        MatrixD RotationMatrix = MatrixD.CreateFromYawPitchRoll(yaw, pitch, roll);

     // Rotate the vertices
        foreach (MyPoint3D Vertex in Vertices) {
            VertexTransform(Vertex, ref RotationMatrix);
        }
    }
}

/** Transform the vertex using a copy of the logic of multiplying the vector by the 
  * transformation matrix which can be found in VRage.Math.Vector3D.
  * This is necessary because the API does not provide a way to update the Vector3Ds
  * in a loop without having to copy their values into and back from intermediary
  * variables a couple of times (once when num1, num2 and num3 are created, and a
  * second time because the compiler doesn't allow passing iterators as "byref"
  * parameters).
  */
private static void VertexTransform(MyPoint3D Vertex, ref MatrixD TransformationMatrix) {
    double num1 = Vertex.X * TransformationMatrix.M11 + Vertex.Y * TransformationMatrix.M21 + Vertex.Z * TransformationMatrix.M31 + TransformationMatrix.M41;
    double num2 = Vertex.X * TransformationMatrix.M12 + Vertex.Y * TransformationMatrix.M22 + Vertex.Z * TransformationMatrix.M32 + TransformationMatrix.M42;
    double num3 = Vertex.X * TransformationMatrix.M13 + Vertex.Y * TransformationMatrix.M23 + Vertex.Z * TransformationMatrix.M33 + TransformationMatrix.M43;
    Vertex.X = num1;
    Vertex.Y = num2;
    Vertex.Z = num3;
}

/////////////////////////////////////////////////////

/**
 * Simple Wavefront OBJ loader capable of loading vertices and faces,
 * without texture coordinates, normals, materials, etc, which are not
 * going to be used in the script anyway
 */
private class MySimpleWavefrontObjLoader {

    public static void LoadFromArray(String[] array, MySimple3DObject Obj3D, int lineStart, int lineEnd) {
        if (array == null || array.Length < 4) {
            throw new ArgumentException("The object definition must contain at least 4 lines (3 vertices and one face)");
        }

        for (int lNumber = lineStart ; lNumber <= lineEnd ; lNumber++) {
            if (lNumber >= array.Length) {
                break;
            }

            String line = array[lNumber];

            char componentType = line.Length == 0 ? 'x' : line[0];

            if (componentType == 'v') {
                String[] vertDef = line.Split(' ');
                if (vertDef.Length >= 4) {
                    try {
                        double x = double.Parse(vertDef[1], System.Globalization.CultureInfo.InvariantCulture);
                        double y = double.Parse(vertDef[2], System.Globalization.CultureInfo.InvariantCulture);
                        double z = double.Parse(vertDef[3], System.Globalization.CultureInfo.InvariantCulture);
                        Obj3D.WithVertex(x,y,z);
                    } catch (Exception exc) {
                        throw new ArgumentException("Cannot read vertex data from [" + line + "]: "  + exc.Message);
                    }
                    
                } else {
                    throw new ArgumentException("Vertex information incomplete at line [" + line + "]");
                }
            }
            else if (componentType == 'f') {
                String[] faceDef = line.Split(' ');
                if (faceDef.Length >= 4) {
                    try {
                        int [] vertIndexes = new int[faceDef.Length - 1];
                        for (int i = 1 ; i < faceDef.Length ; i++) {
                            vertIndexes[i-1] = Int32.Parse(faceDef[i].Split('/')[0]) - 1;
                        }
                        Obj3D.WithFace(vertIndexes);
                    } catch (Exception exc) {
                        throw new ArgumentException("Cannot read face data at line [" + line + "]: " + exc.Message);
                    }
                } else {
                    throw new ArgumentException("Face information incomplete at line [" + line + "]");
                }
            }
        }
    }  
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// APPLICATION INIT ////////////////////////////////////////////////////////////////////////////////////////////////

// This is the resolution of the matrix on the target screen
private const int RES_X = 139, RES_Y =  93;

// This is the String array resulted from loading the Wavefront OBJ
private String[] ObjFileLines;

// The 3D object, which is loaded across multiple iterations to avoid the "Script too Complex" error
private MySimple3DObject Obj3D;

/**
 * This will be the interface to the application.
 */
private MyOnScreenApplication OnScreenApplication;

/**
 * This is the 3D model view which will rendered the 3D object onto
 * the target LCD panel. It is initialized in the InitSprites1() method.
 */
private My3DModelView TheModelView;

/**
 * Counter for the POST page
 */
int currFrame = 0;

/**
  * This is called from the 5th loop
  * It should be used for initializing the application.
  *      > adding pages
  *      > adding components to the pages
  *      > linking logic and animations
  */
private void InitApplication() {
 // Set up the target screen
    TerminalUtils.SetupTextPanelForMatrixDisplay(GridTerminalSystem, TARGET_LCD_PANEL_NAME, 0.190f);

 // Initialized the application with the default POST page
    OnScreenApplication = UiFrameworkUtils.InitSingleScreenApplication(GridTerminalSystem, TARGET_LCD_PANEL_NAME, RES_X, RES_Y, false)
        .WithDefaultPostPage((MyOnScreenApplication app) => {
         // The POST page should disappear after 100 frames
            currFrame++;
            return currFrame >= POST_SCREEN_DURATION;
        })
        .WithoutAutomaticClear() // Don't clear the buffer automatically  - this will only make the 3D model blink
        .WithoutAutomaticFlush() // Don't update the screen automatically - this will also make the 3D model blink
        ;

 // Initialize the main page and add it to the application
    MyPage MainPage = new MyPage();
    OnScreenApplication.AddPage(MainPage);

 // Optionally invert the colors of the main page
    if (INVERT_COLORS) {
        MainPage.WithInvertedColors();
    }

 // Create the model view
    TheModelView = new My3DModelView(OnScreenApplication.GetTargetScreens()[0], INVERT_COLORS)
        .WithRotationSpeeds(ROT_SPEED_RAD_YAW, ROT_SPEED_RAD_PITCH, ROT_SPEED_RAD_ROLL);

 // Attach the 3D object to the model view
    TheModelView.AttachModel(Obj3D);
    TheModelView.SetAttachedModelPosition(0, 0, MODEL_DISTANCE_FROM_VIEW);

 // Add the model view to the main page
    MainPage.AddChild(TheModelView);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// MAIN ////////////////////////////////////////////////////////////////////////////////////////////////////////////

public static MyGridProgram PROGRAM;

public Program() {
    // Set the update speed in milliseconds
    Runtime.UpdateFrequency = UpdateFrequency.Update1;

    // Get a reference to SELF, for debugging from other contexts
    PROGRAM = this;
}

public void Save() {
    // There is no state to be saved
}

private int initStepNbr = 0;

public void Main(string argument, UpdateType updateSource) {
 // Initialize the script. Do it in many steps, to avoid "script too complex" errors
 // caused by loading too many pictures in one single frame.
    if (initStepNbr < 5) {
        initStepNbr++;
        if (initStepNbr == 1) InitSprites1();
        if (initStepNbr == 2) InitSprites2();
        if (initStepNbr == 3) InitSprites3();
        if (initStepNbr == 4) InitSprites4();
        if (initStepNbr == 5) InitApplication();
    } else {
        OnScreenApplication.Cycle();
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////







////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}}

