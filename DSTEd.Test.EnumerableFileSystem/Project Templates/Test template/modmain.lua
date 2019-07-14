PrefabFiles = {
	"fa",
	"M16A4",
	"nato556"
}

Assets = {
    Asset( "IMAGE", "images/saveslot_portraits/fa.tex" ),
    Asset( "ATLAS", "images/saveslot_portraits/fa.xml" ),

    Asset( "IMAGE", "images/selectscreen_portraits/fa.tex" ),
    Asset( "ATLAS", "images/selectscreen_portraits/fa.xml" ),
	
    Asset( "IMAGE", "images/selectscreen_portraits/fa_none.tex" ),
    Asset( "ATLAS", "images/selectscreen_portraits/fa_none.xml" ),
    
    Asset( "IMAGE", "bigportraits/fa.tex" ),
    Asset( "ATLAS", "bigportraits/fa.xml" ),
    
 	Asset( "IMAGE", "bigportraits/fa_none.tex" ),
    Asset( "ATLAS", "bigportraits/fa_none.xml" ),
    
	Asset( "IMAGE", "images/map_icons/fa.tex" ),
	Asset( "ATLAS", "images/map_icons/fa.xml" ),
	
	Asset( "IMAGE", "images/avatars/avatar_fa.tex" ),
    Asset( "ATLAS", "images/avatars/avatar_fa.xml" ),
    ---------------------SND PART----------------
        Asset("SOUNDPACKAGE", "sound/fa.fev"),--
    Asset("SOUND", "sound/m16a4.fsb"),--Sound Bank,只做了开火音效
	--------------------------------------------------
	Asset( "IMAGE", "images/inventoryimages/M16A4.tex" ),
    Asset( "ATLAS", "images/inventoryimages/M16A4.xml" ),

	Asset( "IMAGE", "images/inventoryimages/nato556.tex" ),
    Asset( "ATLAS", "images/inventoryimages/nato556.xml" ),
    
    
    Asset("ANIM","anim/fa.zip"),
    Asset("ANIM","anim/m16a4.zip"),
	Asset("ANIM","anim/nato556.zip"),
	Asset("ANIM","anim/nato556proj.zip"),
}

local require = GLOBAL.require
local STRINGS = GLOBAL.STRINGS
local Ingredient = GLOBAL.Ingredient
local RECIPETABS = GLOBAL.RECIPETABS
local Recipe = GLOBAL.Recipe
local TECH = GLOBAL.TECH

local TUNING = GLOBAL.TUNING
local Player = GLOBAL.ThePlayer
local TheNet = GLOBAL.TheNet
local IsServer = GLOBAL.TheNet:GetIsServer()
local TheInput = GLOBAL.TheInput
local TimeEvent = GLOBAL.TimeEvent
local FRAMES = GLOBAL.FRAMES
local EQUIPSLOTS = GLOBAL.EQUIPSLOTS
local EventHandler = GLOBAL.EventHandler
local SpawnPrefab = GLOBAL.SpawnPrefab
local State = GLOBAL.State
local DEGREES = GLOBAL.DEGREES
local Vector3 = GLOBAL.Vector3
local ACTIONS = GLOBAL.ACTIONS
local FOODTYPE = GLOBAL.FOODTYPE
local PLAYERSTUNLOCK = GLOBAL.PLAYERSTUNLOCK
local GetTime = GLOBAL.GetTime
local HUMAN_MEAT_ENABLED = GLOBAL.HUMAN_MEAT_ENABLED
local TheSim = GLOBAL.TheSim
local ActionHandler = GLOBAL.ActionHandler
GLOBAL.setmetatable(env,{__index=function(t,k) return GLOBAL.rawget(GLOBAL,k) end})

TUNING.whatthefuck =false --M16A4
TUNING.aaaaaaaa = false	--私底下想给作者爽一下bt版，就把这个设置成true


STRINGS.CHARACTER_TITLES.FA = "鸽王"
STRINGS.CHARACTER_NAMES.FA = "Fa鸽"
STRINGS.CHARACTER_DESCRIPTIONS.FA = "咕咕咕"
STRINGS.CHARACTER_QUOTES.FA = "咕咕咕"

--STRINGS.CHARACTERS.FA = require "speech_fa"

STRINGS.NAMES.FA = "Fa鸽"
TUNING.biantaiban=GetModConfigData("biantaiban")

STRINGS.NAMES.NATO556 = "一种子弹"
STRINGS.NAMES.M16A4="M16A4"
---------------------------------------PASS-------------------------------------------
AddRecipe(
	"m16a4",
	{
	Ingredient("goldnugget",3),
	Ingredient("rocks",5),
	Ingredient("flint",4)},
	RECIPETABS.WAR,
	TECH.NONE,
	nil,
	nil,
	nil,
	nil,
	"fa",
	 "images/inventoryimages/M16A4.xml", 
	 "M16A4.tex" 
	)
	STRINGS.NAMES.M16A4 = "M16A4"
STRINGS.RECIPE_DESC.M16A4 = "素质三连发"

AddRecipe(
		"nato556",
		{
			Ingredient("goldnugget",2),
			Ingredient("charcoal",2),
			Ingredient("nitre",1),
		},
			RECIPETABS.WAR,
		TECH.NONE,
		nil,
		nil,
		nil,
		10,
		"fa",
		 "images/inventoryimages/nato556.xml", 
		 "nato556.tex" 
)
-----------------------------------------------------------------------------------


AddMinimapAtlas("images/map_icons/fa.xml")

AddModCharacter("fa", "MALE")

