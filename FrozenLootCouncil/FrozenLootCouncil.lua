
FrozenLootCouncil = LibStub("AceAddon-3.0"):NewAddon("FrozenLootCouncil", "AceConsole-3.0", "AceEvent-3.0")

FrozenLootCouncil:RegisterChatCommand("flc", "FLCSlash")
FrozenLootCouncil.OfficersInRaid = { }
FrozenLootCouncil.RaidersInRaid = { }

function FrozenLootCouncil:OnInitialize()
    
end

function FrozenLootCouncil:OnEnable()
    self:RegisterEvent("CHAT_MSG_WHISPER")
end

function FrozenLootCouncil:OnDisable()
    self:UnregisterEvent("CHAT_MSG_WHISPER")
end

function FrozenLootCouncil:FLCSlash(input)

    -- Todo: Move to config UI
    local expectedGuild = "Frozen"
    local officerRanks = { "Guild Master", "Officer" }
    local raiderRanks = { "Raider", "Veteran", "Alt" }

    local officersInRaid = {}
    local raidersInRaid = {}
    local numOfficers = 0
    local numRaiders = 0

    for i=1, MAX_RAID_MEMBERS do
        if UnitExists("raid"..i) then
            local class,engClass = UnitClass("raid"..i)
            local unitname = UnitName("raid"..i)    
            local guildName, guildRankName, guildRankIndex = GetGuildInfo(unitname);      
            local roll = random(1, 100)

            if guildName == nil then
                guildName = "Unknown guild"
            end
            if guildRankName == nil then
                guildRankName = "Unknown rank"
            end

            if guildName == expectedGuild then
                for _,rank in pairs(officerRanks) do
                    if rank == guildRankName then
                        numOfficers = numOfficers + 1
                        officersInRaid[unitname] = roll
                    end
                end

                for _,rank in pairs(raiderRanks) do
                    if rank == guildRankName then
                        numRaiders = numRaiders + 1
                        raidersInRaid[unitname] = roll
                    end
                end
            end
        end
    end -- loop over raid members

    FrozenLootCouncil.OfficersInRaid = officersInRaid
    FrozenLootCouncil.RaidersInRaid = raidersInRaid

    FrozenLootCouncil:AnnounceLootCouncil()

    -- Todo: Call into RCLootCouncil and set the council directly

end

function FrozenLootCouncil:spairs(t, order)
    -- collect the keys
    local keys = {}
    for k in pairs(t) do keys[#keys+1] = k end

    -- if order function given, sort by it by passing the table and keys a, b,
    -- otherwise just sort the keys 
    if order then
        table.sort(keys, function(a,b) return order(t, a, b) end)
    else
        table.sort(keys)
    end

    -- return the iterator function
    local i = 0
    return function()
        i = i + 1
        if keys[i] then
            return keys[i], t[keys[i]]
        end
    end
end

function FrozenLootCouncil:Trace(message)
    FrozenLootCouncil:Print("FLC Trace: " .. message)
end

function FrozenLootCouncil:Broadcast(message)
    ChatThrottleLib:SendChatMessage("NORMAL", "flc", message, "RAID", nil, nil)
end

function FrozenLootCouncil:AnnounceLootCouncil()
    FrozenLootCouncil:Broadcast("This raids loot council...")
    local i = 1
    for name,roll in FrozenLootCouncil:spairs(FrozenLootCouncil.OfficersInRaid, function(t,a,b) return t[b] < t[a] end) do
        if i > 3 then break end
        FrozenLootCouncil:Broadcast(name .. " (" .. roll .. ")")
        i = i + 1
    end
    i = 1;
    for name,roll in FrozenLootCouncil:spairs(FrozenLootCouncil.RaidersInRaid, function(t,a,b) return t[b] < t[a] end) do
        if i > 3 then break end
        FrozenLootCouncil:Broadcast(name .. " (" .. roll .. ")")
        i = i + 1
    end
    FrozenLootCouncil:Broadcast("Whisper me '!rolls' to see the full set of rolls")
end

function tablelength(T)
    local count = 0
    for _ in pairs(T) do count = count + 1 end
    return count
end

function FrozenLootCouncil:CHAT_MSG_WHISPER(event, text, playerName, languageName, channelName, playerName2, specialFlags, zoneChannelID, channelIndex, channelBaseName, unused, lineID, guid, bnSenderID, isMobile, isSubtitle, hideSenderInLetterbox, supressRaidIcons)

    local rollsRequested = strfind(text, "!rolls")
    
    if rollsRequested ~= nil then

        local allRolls = { }

        for name,roll in pairs(FrozenLootCouncil.OfficersInRaid) do
            allRolls[name] = roll
        end
        for name,roll in pairs(FrozenLootCouncil.RaidersInRaid) do
            allRolls[name] = roll
        end

        for name,roll in FrozenLootCouncil:spairs(allRolls, function(t,a,b) return t[b] < t[a] end) do
            ChatThrottleLib:SendChatMessage("NORMAL", "flc", name .. " (" .. roll .. ")", "WHISPER", nil, playerName)
        end
    end

end