#include maps\mp\_utility;
#include maps\mp\zombies\_zm_utility;
#include maps\mp\zombies\_zm_weapons;
#include common_scripts\utility;
#include maps\mp\gametypes_zm\_hud_util;
#include maps\mp\gametypes_zm\_hud_message;

init()
{
    // 
    precacheshader("zombies_rank_3_ded");

    thread onplayerconnect();
}

onplayerconnect()
{
    for (;;)
    {
        level waittill("connected", player);
        player thread onplayerspawned();
    }
}

onplayerspawned()
{
    self endon("disconnect");
    level endon("end_game");

    for (;;)
    {
        self waittill("spawned_player");

        // 
        self thread get_character();
        self thread show_uwu_text();

        if (!isdefined(self.initial_spawn))
        {
            self.initial_spawn = 1;
        }
    }
}

get_character()
{
    if(self.characterindex == 3)
        self thread BO3_HUD_Marlton();
    else if(self.characterindex == 2)
        self thread BO3_HUD_Misty();
    else if(self.characterindex == 1)
        self thread BO3_HUD_Reporter();
    else if(self.characterindex == 0)
        self thread BO3_HUD_Russman();
}

BO3_HUD_Misty()
{
    level endon("end_game");
    character_hud = newclienthudelem(self);
    character_hud setshader("zombies_rank_3_ded", 31, 31);
    character_hud.alignx = "left";
    character_hud.aligny = "middle";
    character_hud.horzalign = "left";
    character_hud.vertalign = "bottom";
    character_hud.x = -47;
    character_hud.y = -103;
    character_hud.foreground = 1;
    character_hud.hidewheninmenu = 0;
}

BO3_HUD_Marlton()
{
    level endon("end_game");
    character_hud = newclienthudelem(self);
    character_hud setshader("zombies_rank_3_ded", 31, 31);
    character_hud.alignx = "left";
    character_hud.aligny = "middle";
    character_hud.horzalign = "left";
    character_hud.vertalign = "bottom";
    character_hud.x = -47;
    character_hud.y = -103;
    character_hud.foreground = 1;
    character_hud.hidewheninmenu = 0;
}

BO3_HUD_Reporter()
{
    level endon("end_game");
    character_hud = newclienthudelem(self);
    character_hud setshader("zombies_rank_3_ded", 31, 31);
    character_hud.alignx = "left";
    character_hud.aligny = "middle";
    character_hud.horzalign = "left";
    character_hud.vertalign = "bottom";
    character_hud.x = -47;
    character_hud.y = -103;
    character_hud.foreground = 1;
    character_hud.hidewheninmenu = 0;
}

BO3_HUD_Russman()
{
    level endon("end_game");
    character_hud = newclienthudelem(self);
    character_hud setshader("zombies_rank_3_ded", 31, 31);
    character_hud.alignx = "left";
    character_hud.aligny = "middle";
    character_hud.horzalign = "left";
    character_hud.vertalign = "bottom";
    character_hud.x = -47;
    character_hud.y = -103;
    character_hud.foreground = 1;
    character_hud.hidewheninmenu = 0;
}

show_uwu_text()
{
    uwu_text = self createfontstring("default", 1.3);
    uwu_text setpoint("LEFT", "BOTTOM_LEFT", 10, -485);
    uwu_text settext("<<PlayerName>");
    uwu_text.foreground = 1;
    uwu_text.hidewheninmenu = 0;
}
