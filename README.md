# WRobotScripts

A collection of wrobot scripts I have found and modified in various ways to fit my needs for vanilla Wow.


Notes:

For Mages/Priests I have modified wand logic, vanilla wow is difficult to work with for wands specifically.
Copied over the anti-tag logic so that the bot doesnt attack mobs that other players have targetted and are attacking.

Priest:

Added PsychicScream as WowSpell and some logic to use it if surrounded by multiple enemies, doesnt seem to work due to not detecting multiple enemies attacking at once - WIP.
 - EnemiesAttackingMeCount
   Supposed to return how many mobs are attacking the player at the same time.
Added wand logic changes
 - random chance to use wand or smite
 - WandActiveInt function added to try and determine what state the wand is in.
     Its unclear currently what wand states mean, but there are the following states: 0, 1, 2, 3
     Since 'Shoot' can be set as an auto cast, the wand state likely reflects that partially. There must also be states for the cast cooldown, auto cast cancel etc.
