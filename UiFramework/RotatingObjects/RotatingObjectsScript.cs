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
private const string TARGET_LCD_PANEL_NAME = "3D_RENDERING_SCREEN_2";

// false = white foreground on black background
// true  = black foreground on white background
private static bool INVERT_COLORS = false;

// For how many frames should the script display the "INITIALIZING" message on screen
private const int POST_SCREEN_DURATION = 10;

// Distance of the rendered object from the view
//    --- increase this if you get the "Script too Complex" error while rendering
private const double MODEL_DISTANCE_FROM_VIEW = 2;

// Set this to true to re-compute the object's center after loading
private static bool RECENTER_OBJECT_AFTER_LOADING = true;

// Rotation angles in radians (how much should the object turn each frame)
private const double ROT_SPEED_RAD_YAW   = 0.10d;
private const double ROT_SPEED_RAD_PITCH = 0.05d;
private const double ROT_SPEED_RAD_ROLL  = 0.00d;

// Initial rotation angles in radians
private const double INITIAL_ROTATION_RAD_YAW   = 0.00d;
private const double INITIAL_ROTATION_RAD_PITCH = 1.65d;
private const double INITIAL_ROTATION_RAD_ROLL  = 0.00d;


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



// REPLACE THIS DATA WITH THE VERTICES AND FACES DEFINED IN YOUR WAVEFRONT OBJ FILE
// DO NOT DELETE THE = @"
= @"
v -1.192424 2.057751 -0.026655
v -1.169512 1.825120 -0.026655
v -1.101656 1.601430 -0.026655
v -0.991465 1.395275 -0.026655
v -0.843171 1.214579 -0.026655
v -0.662475 1.066286 -0.026655
v -0.456321 0.956094 -0.026655
v -0.232630 0.888238 -0.026655
v 0.000000 0.865326 -0.026655
v 0.232631 0.888238 -0.026655
v 0.456322 0.956094 -0.026655
v 0.662476 1.066286 -0.026655
v 0.843172 1.214580 -0.026655
v 0.991465 1.395276 -0.026655
v 1.101657 1.601430 -0.026655
v 1.169513 1.825121 -0.026655
v 1.192424 2.057752 -0.026655
v 1.169512 2.290382 -0.026655
v 1.101656 2.514073 -0.026655
v 0.991464 2.720227 -0.026655
v 0.843170 2.900923 -0.026655
v 0.662474 3.049216 -0.026655
v 0.456319 3.159408 -0.026655
v 0.232629 3.227263 -0.026655
v 0.259622 3.362970 -0.026655
v 0.509270 3.287241 -0.026655
v 0.739346 3.164263 -0.026655
v 0.941010 2.998763 -0.026655
v 1.106511 2.797099 -0.026655
v 1.229489 2.567023 -0.026655
v 1.305219 2.317376 -0.026655
v 1.330790 2.057752 -0.026655
v 1.305220 1.798127 -0.026655
v 1.229490 1.548480 -0.026655
v 1.106512 1.318404 -0.026655
v 0.941011 1.116740 -0.026655
v 0.739348 0.951239 -0.026655
v 0.509272 0.828261 -0.026655
v 0.259625 0.752531 -0.026655
v 0.000000 0.726960 -0.026655
v -0.259624 0.752531 -0.026655
v -0.509271 0.828261 -0.026655
v -0.739347 0.951239 -0.026655
v -0.941011 1.116740 -0.026655
v -1.106511 1.318403 -0.026655
v -1.229490 1.548479 -0.026655
v -1.305219 1.798126 -0.026655
v -1.330790 2.057751 -0.026655
v -1.305219 2.317375 -0.026655
v -1.229490 2.567022 -0.026655
v -1.106511 2.797098 -0.026655
v -0.941011 2.998761 -0.026655
v -0.739347 3.164262 -0.026655
v -0.509271 3.287240 -0.026655
v -0.259624 3.362970 -0.026655
v 0.000000 3.388541 -0.026655
v -1.169512 2.290381 -0.026655
v -1.101656 2.514072 -0.026655
v -0.991465 2.720226 -0.026655
v -0.843171 2.900922 -0.026655
v -0.662476 3.049215 -0.026655
v -0.456321 3.159407 -0.026655
v -0.232630 3.227263 -0.026655
v 0.000000 3.250175 -0.026655
v 0.300667 2.479588 -0.026655
v 0.305182 2.433748 -0.026655
v 0.318553 2.389669 -0.026655
v 0.340266 2.349046 -0.026655
v 0.369488 2.313440 -0.026655
v 0.405094 2.284218 -0.026655
v 0.445717 2.262505 -0.026655
v 0.489796 2.249134 -0.026655
v 0.535636 2.244619 -0.026655
v 0.581477 2.249134 -0.026655
v 0.625555 2.262505 -0.026655
v 0.666178 2.284218 -0.026655
v 0.701785 2.313440 -0.026655
v 0.731006 2.349046 -0.026655
v 0.752720 2.389669 -0.026655
v 0.766091 2.433748 -0.026655
v 0.770606 2.479588 -0.026655
v 0.766091 2.525429 -0.026655
v 0.752720 2.569507 -0.026655
v 0.731006 2.610130 -0.026655
v 0.701784 2.645737 -0.026655
v 0.666178 2.674958 -0.026655
v 0.625555 2.696671 -0.026655
v 0.581476 2.710042 -0.026655
v 0.305182 2.525428 -0.026655
v 0.318553 2.569507 -0.026655
v 0.340266 2.610130 -0.026655
v 0.369488 2.645736 -0.026655
v 0.405094 2.674958 -0.026655
v 0.445717 2.696671 -0.026655
v 0.489796 2.710042 -0.026655
v 0.535636 2.714557 -0.026655
v -0.771963 2.479588 -0.026655
v -0.767448 2.433748 -0.026655
v -0.754077 2.389669 -0.026655
v -0.732363 2.349046 -0.026655
v -0.703142 2.313440 -0.026655
v -0.667535 2.284218 -0.026655
v -0.626912 2.262505 -0.026655
v -0.582833 2.249134 -0.026655
v -0.536993 2.244619 -0.026655
v -0.491153 2.249134 -0.026655
v -0.447074 2.262505 -0.026655
v -0.406451 2.284218 -0.026655
v -0.370845 2.313440 -0.026655
v -0.341623 2.349046 -0.026655
v -0.319910 2.389669 -0.026655
v -0.306539 2.433748 -0.026655
v -0.302024 2.479588 -0.026655
v -0.306539 2.525429 -0.026655
v -0.319910 2.569507 -0.026655
v -0.341623 2.610130 -0.026655
v -0.370845 2.645737 -0.026655
v -0.406451 2.674958 -0.026655
v -0.447075 2.696671 -0.026655
v -0.491153 2.710042 -0.026655
v -0.767448 2.525428 -0.026655
v -0.754077 2.569507 -0.026655
v -0.732363 2.610130 -0.026655
v -0.703142 2.645736 -0.026655
v -0.667535 2.674958 -0.026655
v -0.626912 2.696671 -0.026655
v -0.582833 2.710042 -0.026655
v -0.536993 2.714557 -0.026655
v -0.738396 1.695023 -0.026655
v 0.737039 1.694527 -0.026655
v -0.738396 1.734901 -0.026655
v 0.737039 1.734405 -0.026655
v -0.222865 1.260578 -0.026655
v 0.221509 1.260573 -0.026655
v 0.221509 1.355347 -0.026655
v -0.222865 1.355352 -0.026655
v 0.491599 1.394593 -0.026655
v 0.491599 1.479682 -0.026655
v -0.492956 1.479983 -0.026655
v -0.492956 1.394895 -0.026655
v -0.075310 1.236359 -0.026655
v 0.073953 1.236359 -0.026655
v 0.073953 1.338285 -0.026655
v -0.075310 1.338285 -0.026655
v 0.363115 1.404033 -0.026655
v 0.363115 1.313873 -0.026655
v -0.364472 1.313935 -0.026655
v -0.364472 1.404095 -0.026655
v 0.599682 1.498203 -0.026655
v 0.688331 1.614233 -0.026655
v 0.688331 1.676345 -0.026655
v 0.599682 1.575579 -0.026655
v -0.601039 1.576247 -0.026655
v -0.689688 1.677045 -0.026655
v -0.689688 1.614932 -0.026655
v -0.601039 1.498870 -0.026655
v -0.991465 1.395275 0.109749
v -1.106511 1.318403 0.109749
v 1.169512 2.290382 0.109749
v 1.305219 2.317376 0.109749
v -0.843171 1.214579 0.109749
v -0.941011 1.116740 0.109749
v 1.101656 2.514073 0.109749
v 1.229489 2.567023 0.109749
v -0.662475 1.066286 0.109749
v -0.739347 0.951239 0.109749
v 0.991464 2.720227 0.109749
v 1.106511 2.797099 0.109749
v -0.456321 0.956094 0.109749
v -0.509271 0.828261 0.109749
v -0.232630 3.227263 0.109749
v -0.259624 3.362970 0.109749
v 0.843170 2.900923 0.109749
v 0.941010 2.998763 0.109749
v -0.232630 0.888238 0.109749
v -0.259624 0.752531 0.109749
v -0.456321 3.159407 0.109749
v -0.509271 3.287240 0.109749
v 0.662474 3.049216 0.109749
v 0.739346 3.164263 0.109749
v 0.000000 0.865326 0.109749
v 0.000000 0.726960 0.109749
v -0.662476 3.049215 0.109749
v -0.739347 3.164262 0.109749
v 0.456319 3.159408 0.109749
v 0.509270 3.287241 0.109749
v 0.232631 0.888238 0.109749
v 0.259625 0.752531 0.109749
v -0.843171 2.900922 0.109749
v -0.941011 2.998761 0.109749
v 0.000000 3.388541 0.109749
v 0.000000 3.250175 0.109749
v 0.232629 3.227263 0.109749
v 0.259622 3.362970 0.109749
v 0.456322 0.956094 0.109749
v 0.509272 0.828261 0.109749
v -0.991465 2.720226 0.109749
v -1.106511 2.797098 0.109749
v 0.662476 1.066286 0.109749
v 0.739348 0.951239 0.109749
v -1.101656 2.514072 0.109749
v -1.229490 2.567022 0.109749
v 0.843172 1.214580 0.109749
v 0.941011 1.116740 0.109749
v -1.169512 2.290381 0.109749
v -1.305219 2.317375 0.109749
v 0.991465 1.395276 0.109749
v 1.106512 1.318404 0.109749
v -1.192424 2.057751 0.109749
v -1.330790 2.057751 0.109749
v 1.101657 1.601430 0.109749
v 1.229490 1.548480 0.109749
v -1.229490 1.548479 0.109749
v -1.101656 1.601430 0.109749
v -1.169512 1.825120 0.109749
v -1.305219 1.798126 0.109749
v 1.330790 2.057752 0.109749
v 1.192424 2.057752 0.109749
v 1.169513 1.825121 0.109749
v 1.305220 1.798127 0.109749
v 0.535636 2.714557 0.109749
v 0.489796 2.710042 0.109749
v 0.445717 2.696671 0.109749
v 0.405094 2.674958 0.109749
v 0.369488 2.645736 0.109749
v 0.340266 2.610130 0.109749
v 0.318553 2.569507 0.109749
v 0.305182 2.525428 0.109749
v 0.300667 2.479588 0.109749
v 0.305182 2.433748 0.109749
v 0.318553 2.389669 0.109749
v 0.340266 2.349046 0.109749
v 0.369488 2.313440 0.109749
v 0.405094 2.284218 0.109749
v 0.445717 2.262505 0.109749
v 0.489796 2.249134 0.109749
v 0.535636 2.244619 0.109749
v 0.581477 2.249134 0.109749
v 0.625555 2.262505 0.109749
v 0.666178 2.284218 0.109749
v 0.701785 2.313440 0.109749
v 0.731006 2.349046 0.109749
v 0.752720 2.389669 0.109749
v 0.766091 2.433748 0.109749
v 0.770606 2.479588 0.109749
v 0.766091 2.525429 0.109749
v 0.752720 2.569507 0.109749
v 0.731006 2.610130 0.109749
v 0.701784 2.645737 0.109749
v 0.666178 2.674958 0.109749
v 0.625555 2.696671 0.109749
v 0.581476 2.710042 0.109749
v -0.582833 2.710042 0.109749
v -0.536993 2.714557 0.109749
v -0.626912 2.696671 0.109749
v -0.667535 2.674958 0.109749
v -0.703142 2.645736 0.109749
v -0.732363 2.610130 0.109749
v -0.754077 2.569507 0.109749
v -0.767448 2.525428 0.109749
v -0.771963 2.479588 0.109749
v -0.767448 2.433748 0.109749
v -0.754077 2.389669 0.109749
v -0.732363 2.349046 0.109749
v -0.703142 2.313440 0.109749
v -0.667535 2.284218 0.109749
v -0.626912 2.262505 0.109749
v -0.582833 2.249134 0.109749
v -0.536993 2.244619 0.109749
v -0.491153 2.249134 0.109749
v -0.447074 2.262505 0.109749
v -0.406451 2.284218 0.109749
v -0.370845 2.313440 0.109749
v -0.341623 2.349046 0.109749
v -0.319910 2.389669 0.109749
v -0.306539 2.433748 0.109749
v -0.302024 2.479588 0.109749
v -0.306539 2.525429 0.109749
v -0.319910 2.569507 0.109749
v -0.341623 2.610130 0.109749
v -0.370845 2.645737 0.109749
v -0.406451 2.674958 0.109749
v -0.447075 2.696671 0.109749
v -0.491153 2.710042 0.109749
v -0.738396 1.734901 0.109749
v -0.738396 1.695023 0.109749
v 0.688331 1.614233 0.109749
v 0.737039 1.694527 0.109749
v 0.737039 1.734405 0.109749
v -0.689688 1.677045 0.109749
v -0.364472 1.313935 0.109749
v -0.222865 1.260578 0.109749
v 0.073953 1.236359 0.109749
v 0.221509 1.260573 0.109749
v 0.363115 1.404033 0.109749
v 0.221509 1.355347 0.109749
v -0.075310 1.338285 0.109749
v -0.222865 1.355352 0.109749
v 0.363115 1.313873 0.109749
v 0.491599 1.394593 0.109749
v 0.599682 1.575579 0.109749
v 0.491599 1.479682 0.109749
v -0.364472 1.404095 0.109749
v -0.492956 1.479983 0.109749
v -0.601039 1.498870 0.109749
v -0.492956 1.394895 0.109749
v -0.075310 1.236359 0.109749
v 0.073953 1.338285 0.109749
v 0.599682 1.498203 0.109749
v 0.688331 1.676345 0.109749
v -0.601039 1.576247 0.109749
v -0.689688 1.614932 0.109749
f 46/1/1 3/2/1 4/3/1 45/4/1
f 32/5/1 17/6/1 18/7/1 31/8/1
f 45/4/1 4/3/1 5/9/1 44/10/1
f 31/8/1 18/7/1 19/11/1 30/12/1
f 44/10/1 5/9/1 6/13/1 43/14/1
f 30/12/1 19/11/1 20/15/1 29/16/1
f 43/14/1 6/13/1 7/17/1 42/18/1
f 56/19/1 64/20/1 63/21/1 55/22/1
f 29/16/1 20/15/1 21/23/1 28/24/1
f 42/18/1 7/17/1 8/25/1 41/26/1
f 55/22/1 63/21/1 62/27/1 54/28/1
f 28/24/1 21/23/1 22/29/1 27/30/1
f 41/26/1 8/25/1 9/31/1 40/32/1
f 54/28/1 62/27/1 61/33/1 53/34/1
f 27/30/1 22/29/1 23/35/1 26/36/1
f 40/32/1 9/31/1 10/37/1 39/38/1
f 53/34/1 61/33/1 60/39/1 52/40/1
f 26/36/1 23/35/1 24/41/1 25/42/1
f 39/38/1 10/37/1 11/43/1 38/44/1
f 52/40/1 60/39/1 59/45/1 51/46/1
f 25/42/1 24/41/1 64/20/1 56/19/1
f 38/44/1 11/43/1 12/47/1 37/48/1
f 51/46/1 59/45/1 58/49/1 50/50/1
f 37/48/1 12/47/1 13/51/1 36/52/1
f 50/50/1 58/49/1 57/53/1 49/54/1
f 36/52/1 13/51/1 14/55/1 35/56/1
f 49/54/1 57/53/1 1/57/1 48/58/1
f 35/56/1 14/55/1 15/59/1 34/60/1
f 48/58/1 1/57/1 2/61/1 47/62/1
f 34/60/1 15/59/1 16/63/1 33/64/1
f 47/62/1 2/61/1 3/2/1 46/1/1
f 33/64/1 16/63/1 17/6/1 32/5/1
f 96/65/1 88/66/1 87/67/1 86/68/1 85/69/1 84/70/1 83/71/1 82/72/1 81/73/1 80/74/1 79/75/1 78/76/1 77/77/1 76/78/1 75/79/1 74/80/1 73/81/1 72/82/1 71/83/1 70/84/1 69/85/1 68/86/1 67/87/1 66/88/1 65/89/1 89/90/1 90/91/1 91/92/1 92/93/1 93/94/1 94/95/1 95/96/1
f 128/97/1 120/98/1 119/99/1 118/100/1 117/101/1 116/102/1 115/103/1 114/104/1 113/105/1 112/106/1 111/107/1 110/108/1 109/109/1 108/110/1 107/111/1 106/112/1 105/113/1 104/114/1 103/115/1 102/116/1 101/117/1 100/118/1 99/119/1 98/120/1 97/121/1 121/122/1 122/123/1 123/124/1 124/125/1 125/126/1 126/127/1 127/128/1
f 150/129/1 151/130/1 132/131/1 130/132/1
f 147/133/1 148/134/1 136/135/1 133/136/1
f 142/137/1 143/138/1 135/139/1 134/140/1
f 146/141/1 145/142/1 138/143/1 137/144/1
f 156/145/1 153/146/1 139/147/1 140/148/1
f 133/136/1 136/135/1 144/149/1 141/150/1
f 141/150/1 144/149/1 143/138/1 142/137/1
f 134/140/1 135/139/1 145/142/1 146/141/1
f 140/148/1 139/147/1 148/134/1 147/133/1
f 137/144/1 138/143/1 152/151/1 149/152/1
f 149/152/1 152/151/1 151/130/1 150/129/1
f 129/153/1 131/154/1 154/155/1 155/156/1
f 155/156/1 154/155/1 153/146/1 156/145/1
f 213/157/2 158/158/2 157/159/2 214/160/2
f 217/161/2 160/162/2 159/163/2 218/164/2
f 158/158/2 162/165/2 161/166/2 157/159/2
f 160/162/2 164/167/2 163/168/2 159/163/2
f 162/165/2 166/169/2 165/170/2 161/166/2
f 164/167/2 168/171/2 167/172/2 163/168/2
f 166/169/2 170/173/2 169/174/2 165/170/2
f 191/175/2 172/176/2 171/177/2 192/178/2
f 168/171/2 174/179/2 173/180/2 167/172/2
f 170/173/2 176/181/2 175/182/2 169/174/2
f 172/176/2 178/183/2 177/184/2 171/177/2
f 174/179/2 180/185/2 179/186/2 173/180/2
f 176/181/2 182/187/2 181/188/2 175/182/2
f 178/183/2 184/189/2 183/190/2 177/184/2
f 180/185/2 186/191/2 185/192/2 179/186/2
f 182/187/2 188/193/2 187/194/2 181/188/2
f 184/189/2 190/195/2 189/196/2 183/190/2
f 186/191/2 194/197/2 193/198/2 185/192/2
f 188/193/2 196/199/2 195/200/2 187/194/2
f 190/195/2 198/201/2 197/202/2 189/196/2
f 194/197/2 191/175/2 192/178/2 193/198/2
f 196/199/2 200/203/2 199/204/2 195/200/2
f 198/201/2 202/205/2 201/206/2 197/202/2
f 200/203/2 204/207/2 203/208/2 199/204/2
f 202/205/2 206/209/2 205/210/2 201/206/2
f 204/207/2 208/211/2 207/212/2 203/208/2
f 206/209/2 210/213/2 209/214/2 205/210/2
f 208/211/2 212/215/2 211/216/2 207/212/2
f 210/213/2 216/217/2 215/218/2 209/214/2
f 212/215/2 220/219/2 219/220/2 211/216/2
f 216/217/2 213/157/2 214/160/2 215/218/2
f 220/219/2 217/161/2 218/164/2 219/220/2
f 221/221/2 222/222/2 223/223/2 224/224/2 225/225/2 226/226/2 227/227/2 228/228/2 229/229/2 230/230/2 231/231/2 232/232/2 233/233/2 234/234/2 235/235/2 236/236/2 237/237/2 238/238/2 239/239/2 240/240/2 241/241/2 242/242/2 243/243/2 244/244/2 245/245/2 246/246/2 247/247/2 248/248/2 249/249/2 250/250/2 251/251/2 252/252/2
f 254/253/2 253/254/2 255/255/2 256/256/2 257/257/2 258/258/2 259/259/2 260/260/2 261/261/2 262/262/2 263/263/2 264/264/2 265/265/2 266/266/2 267/267/2 268/268/2 269/269/2 270/270/2 271/271/2 272/272/2 273/273/2 274/274/2 275/275/2 276/276/2 277/277/2 278/278/2 279/279/2 280/280/2 281/281/2 282/282/2 283/283/2 284/284/2
f 287/285/2 288/286/2 289/287/2 310/288/2
f 291/289/2 292/290/2 298/291/2 303/292/2
f 293/293/2 294/294/2 296/295/2 308/296/2
f 299/297/2 300/298/2 302/299/2 295/300/2
f 305/301/2 306/302/2 304/303/2 311/304/2
f 292/290/2 307/305/2 297/306/2 298/291/2
f 307/305/2 293/293/2 308/296/2 297/306/2
f 294/294/2 299/297/2 295/300/2 296/295/2
f 306/302/2 291/289/2 303/292/2 304/303/2
f 300/298/2 309/307/2 301/308/2 302/299/2
f 309/307/2 287/285/2 310/288/2 301/308/2
f 286/309/2 312/310/2 290/311/2 285/312/2
f 312/310/2 305/301/2 311/304/2 290/311/2
f 80/74/3 81/73/3 245/245/3 244/244/3
f 29/16/4 28/24/4 174/179/4 168/171/4
f 152/151/5 138/143/5 302/299/5 301/308/5
f 81/73/6 82/72/6 246/246/6 245/245/6
f 30/12/7 29/16/7 168/171/7 164/167/7
f 82/72/8 83/71/8 247/247/8 246/246/8
f 31/8/8 30/12/8 164/167/8 160/162/8
f 148/134/9 139/147/9 304/303/9 303/292/9
f 83/71/7 84/70/7 248/248/7 247/247/7
f 32/5/6 31/8/6 160/162/6 217/161/6
f 156/145/10 140/148/10 306/302/10 305/301/10
f 84/70/4 85/69/4 249/249/4 248/248/4
f 33/64/3 32/5/3 217/161/3 220/219/3
f 85/69/11 86/68/11 250/250/11 249/249/11
f 34/60/12 33/64/12 220/219/12 212/215/12
f 133/136/13 141/150/13 307/305/13 292/290/13
f 86/68/14 87/67/14 251/251/14 250/250/14
f 35/56/15 34/60/15 212/215/15 208/211/15
f 141/150/16 142/137/16 293/293/16 307/305/16
f 87/67/17 88/66/17 252/252/17 251/251/17
f 36/52/18 35/56/18 208/211/18 204/207/18
f 135/139/19 143/138/19 308/296/19 296/295/19
f 88/66/20 96/65/20 221/221/20 252/252/20
f 37/48/21 36/52/21 204/207/21 200/203/21
f 143/138/22 144/149/22 297/306/22 308/296/22
f 128/97/23 127/128/23 253/254/23 254/253/23
f 38/44/24 37/48/24 200/203/24 196/199/24
f 127/128/25 126/127/25 255/255/25 253/254/25
f 39/38/26 38/44/26 196/199/26 188/193/26
f 126/127/27 125/126/27 256/256/27 255/255/27
f 40/32/28 39/38/28 188/193/28 182/187/28
f 138/143/29 145/142/29 295/300/29 302/299/29
f 125/126/30 124/125/30 257/257/30 256/256/30
f 41/26/31 40/32/31 182/187/31 176/181/31
f 134/140/32 146/141/32 299/297/32 294/294/32
f 124/125/33 123/124/33 258/258/33 257/257/33
f 42/18/34 41/26/34 176/181/34 170/173/34
f 123/124/35 122/123/35 259/259/35 258/258/35
f 43/14/36 42/18/36 170/173/36 166/169/36
f 140/148/37 147/133/37 291/289/37 306/302/37
f 122/123/38 121/122/38 260/260/38 259/259/38
f 44/10/39 43/14/39 166/169/39 162/165/39
f 136/135/40 148/134/40 303/292/40 298/291/40
f 63/21/28 64/20/28 192/178/28 171/177/28
f 121/122/41 97/121/41 261/261/41 260/260/41
f 45/4/42 44/10/42 162/165/42 158/158/42
f 62/27/26 63/21/26 171/177/26 177/184/26
f 97/121/43 98/120/43 262/262/43 261/261/43
f 46/1/44 45/4/44 158/158/44 213/157/44
f 137/144/45 149/152/45 309/307/45 300/298/45
f 61/33/24 62/27/24 177/184/24 183/190/24
f 98/120/46 99/119/46 263/263/46 262/262/46
f 47/62/46 46/1/46 213/157/46 216/217/46
f 149/152/47 150/129/47 287/285/47 309/307/47
f 60/39/21 61/33/21 183/190/21 189/196/21
f 99/119/44 100/118/44 264/264/44 263/263/44
f 48/58/43 47/62/43 216/217/43 210/213/43
f 132/131/48 151/130/48 310/288/48 289/287/48
f 59/45/18 60/39/18 189/196/18 197/202/18
f 100/118/42 101/117/42 265/265/42 264/264/42
f 49/54/41 48/58/41 210/213/41 206/209/41
f 151/130/49 152/151/49 301/308/49 310/288/49
f 58/49/15 59/45/15 197/202/15 201/206/15
f 101/117/39 102/116/39 266/266/39 265/265/39
f 50/50/38 49/54/38 206/209/38 202/205/38
f 57/53/12 58/49/12 201/206/12 205/210/12
f 102/116/36 103/115/36 267/267/36 266/266/36
f 51/46/35 50/50/35 202/205/35 198/201/35
f 1/57/3 57/53/3 205/210/3 209/214/3
f 103/115/34 104/114/34 268/268/34 267/267/34
f 52/40/33 51/46/33 198/201/33 190/195/33
f 139/147/50 153/146/50 311/304/50 304/303/50
f 2/61/6 1/57/6 209/214/6 215/218/6
f 104/114/31 105/113/31 269/269/31 268/268/31
f 53/34/30 52/40/30 190/195/30 184/189/30
f 153/146/51 154/155/51 290/311/51 311/304/51
f 3/2/8 2/61/8 215/218/8 214/160/8
f 105/113/28 106/112/28 270/270/28 269/269/28
f 54/28/27 53/34/27 184/189/27 178/183/27
f 129/153/52 155/156/52 312/310/52 286/309/52
f 4/3/7 3/2/7 214/160/7 157/159/7
f 106/112/26 107/111/26 271/271/26 270/270/26
f 55/22/25 54/28/25 178/183/25 172/176/25
f 155/156/53 156/145/53 305/301/53 312/310/53
f 5/9/4 4/3/4 157/159/4 161/166/4
f 107/111/24 108/110/24 272/272/24 271/271/24
f 56/19/23 55/22/23 172/176/23 191/175/23
f 6/13/11 5/9/11 161/166/11 165/170/11
f 108/110/21 109/109/21 273/273/21 272/272/21
f 96/65/23 95/96/23 222/222/23 221/221/23
f 7/17/14 6/13/14 165/170/14 169/174/14
f 109/109/18 110/108/18 274/274/18 273/273/18
f 95/96/25 94/95/25 223/223/25 222/222/25
f 8/25/17 7/17/17 169/174/17 175/182/17
f 110/108/15 111/107/15 275/275/15 274/274/15
f 94/95/27 93/94/27 224/224/27 223/223/27
f 9/31/20 8/25/20 175/182/20 181/188/20
f 111/107/12 112/106/12 276/276/12 275/275/12
f 93/94/30 92/93/30 225/225/30 224/224/30
f 10/37/23 9/31/23 181/188/23 187/194/23
f 112/106/3 113/105/3 277/277/3 276/276/3
f 92/93/33 91/92/33 226/226/33 225/225/33
f 11/43/25 10/37/25 187/194/25 195/200/25
f 113/105/6 114/104/6 278/278/6 277/277/6
f 91/92/35 90/91/35 227/227/35 226/226/35
f 12/47/27 11/43/27 195/200/27 199/204/27
f 114/104/8 115/103/8 279/279/8 278/278/8
f 90/91/38 89/90/38 228/228/38 227/227/38
f 13/51/30 12/47/30 199/204/30 203/208/30
f 115/103/7 116/102/7 280/280/7 279/279/7
f 89/90/41 65/89/41 229/229/41 228/228/41
f 14/55/33 13/51/33 203/208/33 207/212/33
f 116/102/4 117/101/4 281/281/4 280/280/4
f 65/89/43 66/88/43 230/230/43 229/229/43
f 15/59/35 14/55/35 207/212/35 211/216/35
f 117/101/11 118/100/11 282/282/11 281/281/11
f 66/88/46 67/87/46 231/231/46 230/230/46
f 16/63/38 15/59/38 211/216/38 219/220/38
f 118/100/14 119/99/14 283/283/14 282/282/14
f 67/87/44 68/86/44 232/232/44 231/231/44
f 17/6/41 16/63/41 219/220/41 218/164/41
f 119/99/17 120/98/17 284/284/17 283/283/17
f 68/86/42 69/85/42 233/233/42 232/232/42
f 18/7/43 17/6/43 218/164/43 159/163/43
f 120/98/20 128/97/20 254/253/20 284/284/20
f 69/85/39 70/84/39 234/234/39 233/233/39
f 19/11/46 18/7/46 159/163/46 163/168/46
f 131/154/54 129/153/54 286/309/54 285/312/54
f 70/84/36 71/83/36 235/235/36 234/234/36
f 20/15/44 19/11/44 163/168/44 167/172/44
f 150/129/55 130/132/55 288/286/55 287/285/55
f 71/83/34 72/82/34 236/236/34 235/235/34
f 21/23/42 20/15/42 167/172/42 173/180/42
f 130/132/56 132/131/56 289/287/56 288/286/56
f 72/82/31 73/81/31 237/237/31 236/236/31
f 22/29/39 21/23/39 173/180/39 179/186/39
f 154/155/57 131/154/57 285/312/57 290/311/57
f 73/81/28 74/80/28 238/238/28 237/237/28
f 23/35/36 22/29/36 179/186/36 185/192/36
f 147/133/58 133/136/58 292/290/58 291/289/58
f 74/80/26 75/79/26 239/239/26 238/238/26
f 24/41/34 23/35/34 185/192/34 193/198/34
f 142/137/59 134/140/59 294/294/59 293/293/59
f 75/79/24 76/78/24 240/240/24 239/239/24
f 64/20/31 24/41/31 193/198/31 192/178/31
f 145/142/60 135/139/60 296/295/60 295/300/60
f 76/78/21 77/77/21 241/241/21 240/240/21
f 25/42/20 56/19/20 191/175/20 194/197/20
f 144/149/61 136/135/61 298/291/61 297/306/61
f 77/77/18 78/76/18 242/242/18 241/241/18
f 26/36/17 25/42/17 194/197/17 186/191/17
f 78/76/15 79/75/15 243/243/15 242/242/15
f 27/30/14 26/36/14 186/191/14 180/185/14
f 79/75/12 80/74/12 244/244/12 243/243/12
f 28/24/11 27/30/11 180/185/11 174/179/11
f 146/141/62 137/144/62 300/298/62 299/297/62
"
// DO NOT DELETE THE DOUBLE QUOTES AT THE END
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

private void InitSprites5() {
    if (RECENTER_OBJECT_AFTER_LOADING) {
        Obj3D.Recenter();
    }

    Obj3D.Rotate(INITIAL_ROTATION_RAD_YAW, INITIAL_ROTATION_RAD_PITCH, INITIAL_ROTATION_RAD_ROLL);
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
  * Applies the given transformation matrix to all the vertices owned by the object
  */
    public void Transform(MatrixD TransformationMatrix) {
        foreach (MyPoint3D Vertex in Vertices) {
            VertexTransform(Vertex, ref TransformationMatrix);
        }
    }

/**
  * Rotates all the vertices of the object using Vrage.Math, which
  * is provided by the Space Engineers API.
  */
    public void Rotate(double yaw, double pitch, double roll) {
     // Create the rotation matrix
        MatrixD RotationMatrix = MatrixD.CreateFromYawPitchRoll(yaw, pitch, roll);

     // Rotate the vertices
        Transform(RotationMatrix);
    }

/**
  * Translates all the vertices of the object using Vrage.Math, which
  * is provided by the Space Engineers API.
  */
    public void Translate(double x, double y, double z) {
     // Create the translation matrix
        MatrixD TranslationMatrix = MatrixD.CreateTranslation(x, y, z);

     // Translate the vertices
        Transform(TranslationMatrix);
    }

/**
  * Translates all the vertices of the object so that the center point
  * resides half way between the positive corner and the negative corner
  */
    public void Recenter() {
     // See how much the object needs to be translated in each direction
     // to have the object's center half way between the positive corner
     // and the negative corner
        double xCorrection = ComputeCorrection(PositiveCorner.X, NegativeCorner.X);
        double yCorrection = ComputeCorrection(PositiveCorner.Y, NegativeCorner.Y);
        double zCorrection = ComputeCorrection(PositiveCorner.Z, NegativeCorner.Z);

     // Translate the object's vertices by the computed amount
        Translate(xCorrection, yCorrection, zCorrection);
    }

    private double ComputeCorrection(double lowVal, double highVal) {
        double halfPoint = (highVal - lowVal) / 2;
        return halfPoint - highVal;
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
    if (initStepNbr < 6) {
        initStepNbr++;
        if (initStepNbr == 1) InitSprites1();
        if (initStepNbr == 2) InitSprites2();
        if (initStepNbr == 3) InitSprites3();
        if (initStepNbr == 4) InitSprites4();
        if (initStepNbr == 5) InitSprites5();
        if (initStepNbr == 6) InitApplication();
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

