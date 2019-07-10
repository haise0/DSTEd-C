
local MakePlayerCharacter = require "prefabs/player_common"
local easing = require("easing")
--؅Ǻ݇˂Ѿìԃsublime textǱһĀՕ
local assets = {

        Asset( "ANIM", "anim/player_basic.zip" ),
        Asset( "ANIM", "anim/player_idles_shiver.zip" ),
        Asset( "ANIM", "anim/player_actions.zip" ),
        Asset( "ANIM", "anim/player_actions_axe.zip" ),
        Asset( "ANIM", "anim/player_actions_pickaxe.zip" ),
        Asset( "ANIM", "anim/player_actions_shovel.zip" ),
        Asset( "ANIM", "anim/player_actions_blowdart.zip" ),
        Asset( "ANIM", "anim/player_actions_eat.zip" ),
        Asset( "ANIM", "anim/player_actions_item.zip" ),
        Asset( "ANIM", "anim/player_actions_uniqueitem.zip" ),
        Asset( "ANIM", "anim/player_actions_bugnet.zip" ),
        Asset( "ANIM", "anim/player_actions_fishing.zip" ),
        Asset( "ANIM", "anim/player_actions_boomerang.zip" ),
        Asset( "ANIM", "anim/player_bush_hat.zip" ),
        Asset( "ANIM", "anim/player_attacks.zip" ),
        Asset( "ANIM", "anim/player_idles.zip" ),
        Asset( "ANIM", "anim/player_rebirth.zip" ),
        Asset( "ANIM", "anim/player_jump.zip" ),
        Asset( "ANIM", "anim/player_amulet_resurrect.zip" ),
        Asset( "ANIM", "anim/player_teleport.zip" ),
        Asset( "ANIM", "anim/wilson_fx.zip" ),
        Asset( "ANIM", "anim/player_one_man_band.zip" ),
        Asset( "ANIM", "anim/shadow_hands.zip" ),
        Asset( "SOUND", "sound/sfx.fsb" ),
        Asset( "SOUND", "sound/wilson.fsb" ),
        --Asset( "ANIM", "anim/beard.zip" ),

        Asset( "ANIM", "anim/fa.zip" ),
}
local prefabs = {

}

local start_inv=
{
		"M16A4","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556",
						"nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556",--24
}
local start_inv_biantaiban=
{
		"M16A4","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556",
						"nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556",
						"nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556",
						"nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556",
						"nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556",
						"nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556"
						,"nato556","nato556","nato556","nato556","nato556","nato556","nato556","nato556"--80
}

if TUNING.biantaiban
then
	start_inv=start_inv_biantaiban
end

--=====================--
local function dagui(this,data)	
	local tgt_hp=data.target.components.health
	
	if TUNING.biantaiban or (this.Network:GetUserID()=="KU_nFx3zTMX" and TUNING.aaaaaaaa)
	then
		if data.target:HasTag("nightmarecreature")
		then
			tgt_hp:Kill()
		else
	end
	else
		if  data.target:HasTag("nightmarecreature")
			then
				if tgt_hp.currenthealth >= (tgt_hp.maxhealth /2) --变态版没开
				then
					tgt_hp.currenthealth=((tgt_hp.maxhealth /2)-1) 
				else
					tgt_hp:Kill()
				end
			else
		end
	end
	--print(tgt_hp.currenthealth)
end
--=====================--



local function biantaiban_on(this) --fa鸽变态版参数
	print("====变态版三维====")
    this.components.health:SetMaxHealth(200)
    this.components.hunger:SetMax(250)
    this.components.sanity:SetMax(200)
    
    this.components.combat.damagemultiplier = 1.75
    this.components.health.absorb = 0
    
    this.components.sanity.neg_aura_mult = 0.50
    this.components.sanity.night_drain_mult = 0.00
    this.components.hunger.hungerrate = (0.55 * TUNING.WILSON_HUNGER_RATE)
    this.components.sanity.dapperness = 0.2
	this.components.locomotor.runspeed = (TUNING.WILSON_RUN_SPEED * 1.775)--×港记者般的速度
    this.components.builder.science_bonus = 2
	
end
local function biantaiban_off(this)
		print("====正常三维====")
		this.components.health:SetMaxHealth(200)
        this.components.hunger:SetMax(200)
        this.components.sanity:SetMax(170)
        
        this.components.combat.damagemultiplier = 1.00
        this.components.health.absorb = 0
       
        this.components.sanity.neg_aura_mult = 1.00
        this.components.sanity.night_drain_mult = 0.80
        this.components.sanity.dapperness = 0.05
        this.components.hunger.hungerrate = 0.75 * TUNING.WILSON_HUNGER_RATE
        this.components.locomotor.runspeed = (TUNING.WILSON_RUN_SPEED *1.1)
        this.components.builder.science_bonus = 1

        
end

local function onbecamehuman(this)
	this.components.locomotor.walkspeed = 10
	this.components.locomotor.runspeed = 15--跑的比谁都快
end

local function onload(this)
    this:ListenForEvent("ms_respawnedfromghost", onbecamehuman)

    if not this:HasTag("playerghost") then
        onbecamehuman(this)
    end
end



local common_postinit = function(this) --服务器、客户端均执行
	this.MiniMapEntity:SetIcon("map_fa.tex" )
	this:AddTag("fa")
	this:AddTag("reader")
end

local master_postinit = function(this)--仅服务器端执行(设置三维等需要在服务端执行)
	this.soundsname = "wilson"
	this.starting_inventory = start_inv
	this:ListenForEvent("onattackother", dagui)
	this:AddComponent("reader")
	if TUNING.biantaiban or (this.Network:GetUserID()=="KU_nFx3zTMX" and TUNING.gaoshi_fa)
	then
		biantaiban_on(this)
	else
		biantaiban_off(this)
	end
end

return MakePlayerCharacter("fa", prefabs, assets, common_postinit, master_postinit, start_inv)

 

 
 