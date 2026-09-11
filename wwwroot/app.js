const API_URL = 'http://localhost:8080/api';
let token = localStorage.getItem('token') || '';
let currentSetupId = null;
let currentEvolutionId = null;
let battleState = {
    playerTeam: [],
    activeIndex: 0,
    enemyPoke: null,
    isWild: false,
    npcTeam: [],
    currentNpcIdx: 0,
    switchingAfterFaint: false
};

if (token) showScreen('menu-screen');
const types = ["Normal", "Fire", "Water", "Grass", "Electric", "Ice", "Fighting", "Poison", "Ground", "Flying", "Psychic", "Bug", "Rock", "Ghost", "Dragon", "Dark", "Steel", "Fairy"];
const archetypes = ["Bug Catcher", "Fire Tamer", "Swimmer", "Dragon Tamer", "Youngster", "Lass", "Psychic", "Hiker", "Bird Keeper", "Super Nerd", "Blackbelt", "Team Rocket Grunt"];

function showScreen(id) { document.querySelectorAll('.screen').forEach(s => s.classList.remove('active')); document.getElementById(id).classList.add('active'); }
function showRegister() { document.querySelector('#auth-screen .box').style.display = 'none'; document.getElementById('register-box').style.display = 'block'; }
function showLogin() { document.querySelector('#auth-screen .box').style.display = 'block'; document.getElementById('register-box').style.display = 'none'; }

async function req(method, url, data = null) {
    const opt = { method, headers: { 'Content-Type': 'application/json' } };
    if (token) opt.headers['Authorization'] = `Bearer ${token}`;
    if (data) opt.body = JSON.stringify(data);
    const res = await fetch(`${API_URL}${url}`, opt);
    if (!res.ok) { const text = await res.text(); let errorMsg = `Error ${res.status}`; try { const e = JSON.parse(text); if (Array.isArray(e)) errorMsg = e.map(err => err.description || JSON.stringify(err)).join('\n'); else if (e.errors) errorMsg = Object.values(e.errors).flat().join('\n'); else if (e.message) errorMsg = e.message; } catch { } throw new Error(errorMsg); }
    return res.json();
}

async function login() { try { const r = await req('POST', '/auth/login', { username: document.getElementById('login-username').value, password: document.getElementById('login-password').value }); token = r.token; localStorage.setItem('token', token); showScreen('menu-screen'); } catch (e) { alert(e.message); } }
async function register() { try { const u = document.getElementById('reg-username').value; const p = document.getElementById('reg-password').value; if (p !== document.getElementById('reg-confirm-password').value) { alert('Passwords do not match.'); return; } await req('POST', '/auth/register', { username: u, password: p }); showLogin(); alert('Registered!'); } catch (e) { alert(e.message); } }
function logout() { token = ''; localStorage.removeItem('token'); showScreen('auth-screen'); }

async function loadTeam() {
    try {
        const team = await req('GET', '/team');
        if (team.length === 0) { showScreen('starter-screen'); return; }
        document.getElementById('team-list').innerHTML = team.map(p => `
            <div class="team-pokemon-row">
                <div class="team-pokemon-info" style="cursor: pointer;" onclick="openSetup(${p.id}, '${p.nickname.replace(/'/g, "\\'")}')">
                    <strong>${p.nickname}</strong> (Lv. ${p.level}) - ${p.gender}<br>
                    <small>${p.primaryType}${p.secondaryType ? '/' + p.secondaryType : ''} | Ability: ${p.ability ? p.ability.name : 'None'}</small><br>
                    <small>Moves: ${p.moves.map(m => m.name).join(', ') || 'None'}</small>
                </div>
                <div class="team-evolve-box">
                    <button class="action-btn" onclick="event.stopPropagation(); checkEvolution(${p.id}, '${p.nickname.replace(/'/g, "\\'")}')">Evolve</button>
                </div>
            </div>`).join('');
    } catch (e) { alert(e.message); }
}

function getMoveEffectText(m) {
    if (!m.effectType || m.effectType === "None") return "";
    let t = "";
    if (m.effectType === "StatChange" && m.targetStat) {
        let target = m.stageChange > 0 ? "↑" : "↓";
        t = `${target} ${m.targetStat} by ${Math.abs(m.stageChange)}`;
    }
    else if (m.effectType === "ApplyStatus" && m.applyStatus) t = `Inflicts ${m.applyStatus}`;
    else if (m.effectType === "Heal") t = `Heals ${m.power}% HP`;
    if (m.secondaryEffectChance) t += ` (${m.secondaryEffectChance}%)`;
    return t;
}

function getAbilityEffectText(a) {
    if (!a.trigger || a.trigger === "None") return "";
    let t = `[${a.trigger}] `;
    if (a.statusInteraction) t += `Interacts with ${a.statusInteraction} status`;
    else if (a.effectType === "StatChange" && a.targetStat) t += `Modifies ${a.targetStat} by ${a.stageChange}`;
    return t;
}

async function openSetup(id, name) {
    currentSetupId = id; document.getElementById('setup-pokemon-name').textContent = name;
    document.getElementById('team-list').style.display = 'none';
    document.getElementById('team-back-btn').style.display = 'none';
    document.getElementById('setup-box').style.display = 'block';
    try {
        const opts = await req('GET', `/team/options/${id}`);
        document.getElementById('setup-moves').innerHTML = opts.availableMoves.map(m => `<label style="display:flex;gap:8px;margin-bottom:8px;padding:5px;background:#111;border-radius:4px;"><input type="checkbox" class="move-cb" value="${m.id}"><span><strong>${m.name}</strong><br><small>${m.type} | ${m.category} | Pwr:${m.power} | Acc:${m.accuracy} | PP:${m.pp}</small>${getMoveEffectText(m) ? `<br><small style="color:#e63946">${getMoveEffectText(m)}</small>` : ''}</span></label>`).join('');
        document.getElementById('setup-abilities').innerHTML = opts.availableAbilities.map(a => `<label style="display:flex;gap:8px;margin-bottom:8px;padding:5px;background:#111;border-radius:4px;"><input type="radio" name="ability" value="${a.id}"><span><strong>${a.name}</strong>${getAbilityEffectText(a) ? `<br><small style="color:#e63946">${getAbilityEffectText(a)}</small>` : ''}</span></label>`).join('');
    } catch (e) { alert(e.message); cancelSetup(); }
}

async function saveSetup() {
    const mIds = Array.from(document.querySelectorAll('.move-cb:checked')).map(cb => parseInt(cb.value));
    if (mIds.length === 0 || mIds.length > 4) { alert("Select 1-4 moves."); return; }
    const aId = document.querySelector('input[name="ability"]:checked')?.value || null;
    try { await req('PUT', `/team/setup/${currentSetupId}`, { selectedMoveIds: mIds, selectedAbilityId: aId ? parseInt(aId) : null, selectedItemId: null }); alert("Saved!"); cancelSetup(); loadTeam(); } catch (e) { alert(e.message); }
}
function cancelSetup() { document.getElementById('setup-box').style.display = 'none'; document.getElementById('team-list').style.display = 'block'; document.getElementById('team-back-btn').style.display = 'block'; }

async function checkEvolution(id, name) {
    try {
        const result = await req('GET', `/evolution/check/${id}`);
        if (!result.canEvolve) { alert(`${name} cannot evolve right now.`); return; }
        currentEvolutionId = id;
        document.getElementById('evolution-pokemon-name').textContent = name;
        document.getElementById('team-list').style.display = 'none';
        document.getElementById('team-back-btn').style.display = 'none';
        document.getElementById('evolution-box').style.display = 'block';
        document.getElementById('evolution-options').innerHTML = result.options.map(opt => `
            <div class="list-item" style="margin-bottom: 10px;">
                <strong>${opt.targetName}</strong><br>
                <small>Required Level: ${opt.requiredLevel}</small><br>
                <button class="action-btn" style="margin-top: 8px;" onclick="executeEvolution(${opt.targetSpeciesId}, '${opt.targetName}')">Evolve into ${opt.targetName}</button>
            </div>
        `).join('');
    } catch (e) { alert(e.message); }
}

async function executeEvolution(targetSpeciesId, targetName) {
    if (!confirm(`Evolve into ${targetName}?`)) return;
    try {
        await req('POST', `/evolution/execute/${currentEvolutionId}?targetSpeciesId=${targetSpeciesId}`);
        alert(`Evolved into ${targetName}!`);
        cancelEvolution();
        loadTeam();
    } catch (e) { alert(e.message); }
}

function cancelEvolution() {
    document.getElementById('evolution-box').style.display = 'none';
    document.getElementById('team-list').style.display = 'block';
    document.getElementById('team-back-btn').style.display = 'block';
}

async function selectStarter(id) { try { await req('POST', '/battle/catch', { speciesId: id, level: 5, currentHp: 1, maxHp: 100, gender: 1 }); alert('Starter joined!'); showScreen('team-screen'); loadTeam(); } catch (e) { alert(e.message); } }

async function loadStorage() {
    try {
        const [team, storage] = await Promise.all([req('GET', '/team'), req('GET', '/team/storage')]);
        document.getElementById('team-storage-list').innerHTML = team.length === 0 ? '<p>Empty</p>' : team.map(p => `<div class="list-item"><strong>${p.nickname}</strong> (Lv.${p.level})<br><button class="action-btn" onclick="swapPc(${p.id}, false)">Move to PC</button></div>`).join('');
        document.getElementById('pc-storage-list').innerHTML = storage.length === 0 ? '<p>Empty</p>' : storage.map(p => `<div class="list-item"><strong>${p.nickname}</strong> (Lv.${p.level})<br><button class="action-btn" onclick="swapPc(${p.id}, true)">Move to Team</button> <button class="action-btn" onclick="releasePokemon(${p.id})">Release</button></div>`).join('');
    } catch (e) { alert(e.message); }
}
async function swapPc(id, toTeam) { try { await req('PUT', `/team/swap/${id}?addToTeam=${toTeam}`); loadStorage(); } catch (e) { alert(e.message); } }
async function releasePokemon(id) { if (!confirm('Release?')) return; try { await req('DELETE', `/team/release/${id}`); loadStorage(); } catch (e) { alert(e.message); } }
async function loadTypes() { const s = document.getElementById('type-select'); if (s.options.length > 1) return; types.forEach(t => { const o = document.createElement('option'); o.value = t; o.textContent = t; s.appendChild(o); }); }

async function findWild() {
    try {
        const type = document.getElementById('type-select').value; if (!type) return;
        const wild = await req('GET', `/battle/wild?type=${type}`);
        const team = await req('GET', '/team');
        if (team.length === 0) { alert("You need a team to battle!"); return; }
        battleState.isWild = true;
        battleState.npcTeam = [];
        battleState.currentNpcIdx = 0;
        battleState.playerTeam = team.map(p => mapToBattlePoke(p));
        battleState.activeIndex = 0;
        battleState.enemyPoke = mapWildToBattlePoke(wild);
        battleState.switchingAfterFaint = false;
        initBattleUI();
    } catch (e) { alert(e.message); }
}

async function findTrainer() {
    try {
        const arch = archetypes[Math.floor(Math.random() * archetypes.length)];
        document.getElementById('trainer-title').textContent = `${arch} wants to battle!`;
        document.getElementById('trainer-info').innerHTML = 'Loading...';
        const data = await req('GET', `/battle/trainer?archetype=${encodeURIComponent(arch)}`);
        const team = await req('GET', '/team');
        if (team.length === 0) { alert("You need a team to battle!"); return; }
        battleState.isWild = false;
        battleState.npcTeam = data.team;
        battleState.currentNpcIdx = 0;
        battleState.playerTeam = team.map(p => mapToBattlePoke(p));
        battleState.activeIndex = 0;
        battleState.enemyPoke = mapNpcToBattlePoke(data.team[0]);
        battleState.switchingAfterFaint = false;
        initBattleUI();
    } catch (e) { alert(e.message); }
}

function mapToBattlePoke(p) {
    const hp = Math.floor(((2 * p.baseHp * p.level) / 100) + p.level + 10);
    let pTypes = [p.primaryType];
    if (p.secondaryType) pTypes.push(p.secondaryType);
    let moves = p.moves && p.moves.length > 0 ? p.moves : [{ id: 999, name: "Struggle", type: "Normal", category: "Physical", power: 50, accuracy: 100, pp: 10, effectType: "None" }];
    return {
        id: p.id, name: p.nickname, level: p.level, types: pTypes, currentHp: hp, maxHp: hp,
        baseAtk: p.baseAttack, baseDef: p.baseDefense, baseSpA: p.baseSpecialAttack, baseSpD: p.baseSpecialDefense, baseSpd: p.baseSpeed,
        stages: { Attack: 0, Defense: 0, SpecialAttack: 0, SpecialDefense: 0, Speed: 0 },
        status: "None", statusTurns: 0, moves: moves, ability: p.ability
    };
}

function mapWildToBattlePoke(w) {
    let wTypes = [w.primaryType];
    if (w.secondaryType) wTypes.push(w.secondaryType);
    let wildMoves = w.moves && w.moves.length > 0
        ? w.moves
        : [{ id: 999, name: "Struggle", type: "Normal", category: "Physical", power: 50, accuracy: 100, pp: 10, effectType: "None" }];
    return {
        id: w.speciesId, name: w.name, level: w.level, types: wTypes, currentHp: w.maxHp, maxHp: w.maxHp,
        baseAtk: w.baseAttack, baseDef: w.baseDefense, baseSpA: w.baseSpecialAttack, baseSpD: w.baseSpecialDefense, baseSpd: w.baseSpeed,
        stages: { Attack: 0, Defense: 0, SpecialAttack: 0, SpecialDefense: 0, Speed: 0 },
        status: "None", statusTurns: 0, moves: wildMoves, ability: w.ability || null
    };
}

function mapNpcToBattlePoke(n) {
    let nTypes = [n.primaryType];
    if (n.secondaryType) nTypes.push(n.secondaryType);
    return {
        id: n.speciesId, name: n.name, level: n.level, types: nTypes, currentHp: n.maxHp, maxHp: n.maxHp,
        baseAtk: n.baseAttack, baseDef: n.baseDefense, baseSpA: n.baseSpecialAttack, baseSpD: n.baseSpecialDefense, baseSpd: n.baseSpeed,
        stages: { Attack: 0, Defense: 0, SpecialAttack: 0, SpecialDefense: 0, Speed: 0 },
        status: "None", statusTurns: 0, moves: n.moves, ability: n.ability || null
    };
}

function getActivePokemon() {
    return battleState.playerTeam[battleState.activeIndex];
}

function getAliveTeam() {
    return battleState.playerTeam.filter(p => p.currentHp > 0);
}

function getEffectiveStat(pokemon, statName) {
    let baseStat;
    switch (statName) {
        case "Attack": baseStat = pokemon.baseAtk; break;
        case "Defense": baseStat = pokemon.baseDef; break;
        case "SpecialAttack": baseStat = pokemon.baseSpA; break;
        case "SpecialDefense": baseStat = pokemon.baseSpD; break;
        case "Speed": baseStat = pokemon.baseSpd; break;
        default: return baseStat;
    }
    let stage = pokemon.stages[statName];
    let multiplier = stage > 0 ? (2 + stage) / 2 : stage < 0 ? 2 / (2 - stage) : 1;
    if (statName === "Speed" && pokemon.status === "Paralyze") { multiplier *= 0.5; }
    return Math.floor(baseStat * multiplier);
}

function applyStartOfTurnEffects(pokemon) {
    if (pokemon.status === "None") return "ok";
    if (pokemon.status === "Freeze") {
        if (Math.random() < 0.2) { pokemon.status = "None"; pokemon.statusTurns = 0; addLog(`${pokemon.name} thawed out!`); return "ok"; }
        addLog(`${pokemon.name} is frozen solid and can't move!`); return "cant_move";
    }
    if (pokemon.status === "Sleep") {
        pokemon.statusTurns--;
        if (pokemon.statusTurns <= 0) { pokemon.status = "None"; addLog(`${pokemon.name} woke up!`); return "ok"; }
        addLog(`${pokemon.name} is fast asleep! (${pokemon.statusTurns} turns remaining)`); return "cant_move";
    }
    let dmg = 0;
    if (pokemon.status === "Burn") { dmg = Math.max(1, Math.floor(pokemon.maxHp / 16)); addLog(`${pokemon.name} is hurt by its burn! (-${dmg} HP)`); }
    if (pokemon.status === "Poison") { dmg = Math.max(1, Math.floor(pokemon.maxHp / 8)); addLog(`${pokemon.name} is hurt by poison! (-${dmg} HP)`); }
    if (pokemon.status === "Toxic") { pokemon.statusTurns++; dmg = Math.max(1, Math.floor(pokemon.maxHp * pokemon.statusTurns / 16)); addLog(`${pokemon.name} is hurt by toxic! (-${dmg} HP, counter: ${pokemon.statusTurns})`); }
    pokemon.currentHp = Math.max(0, pokemon.currentHp - dmg);
    if (pokemon.currentHp <= 0) { addLog(`${pokemon.name} fainted from ${pokemon.status.toLowerCase()}!`); return "fainted"; }
    return "ok";
}

function applyMoveEffect(attacker, defender, move) {
    addLog(`${attacker.name} used ${move.name}!`);
    if (!move.effectType || move.effectType === "None") { addLog("But nothing happened!"); return; }
    if (move.effectType === "StatChange" && move.targetStat) {
        let target = move.stageChange > 0 ? attacker : defender;
        let stages = move.stageChange;
        let statName = move.targetStat;
        let oldStage = target.stages[statName];
        target.stages[statName] = Math.max(-6, Math.min(6, target.stages[statName] + stages));
        let actualChange = target.stages[statName] - oldStage;
        if (actualChange === 0) { addLog(`${target.name}'s ${statName} won't go any ${stages > 0 ? 'higher' : 'lower'}!`); }
        else { let effectiveStat = getEffectiveStat(target, statName); let direction = actualChange > 0 ? "rose" : "fell"; addLog(`${target.name}'s ${statName} ${direction} to stage ${target.stages[statName]}! (Effective: ${effectiveStat})`); }
    }
    if (move.effectType === "ApplyStatus" && move.applyStatus && move.applyStatus !== "None") {
        if (defender.status !== "None") { addLog(`But ${defender.name} is already ${defender.status.toLowerCase()}!`); return; }
        let status = move.applyStatus;
        if (status === "Burn" && defender.types.includes("Fire")) { addLog(`Fire types can't be burned!`); return; }
        if ((status === "Poison" || status === "Toxic") && (defender.types.includes("Poison") || defender.types.includes("Steel"))) { addLog(`It doesn't affect ${defender.name}...`); return; }
        if (status === "Paralyze" && defender.types.includes("Electric")) { addLog(`Electric types can't be paralyzed!`); return; }
        if (status === "Freeze" && defender.types.includes("Ice")) { addLog(`Ice types can't be frozen!`); return; }
        if (move.accuracy < 100 && Math.random() * 100 >= move.accuracy) { addLog(`${attacker.name}'s attack missed!`); return; }
        defender.status = status;
        defender.statusTurns = 0;
        if (status === "Sleep") { defender.statusTurns = Math.floor(Math.random() * 3) + 1; addLog(`${defender.name} fell asleep! (${defender.statusTurns} turns)`); }
        else if (status === "Toxic") { addLog(`${defender.name} was badly poisoned!`); }
        else { addLog(`${defender.name} is now ${status.toLowerCase()}!`); }
    }
    if (move.effectType === "Heal") {
        let healAmount = Math.floor(attacker.maxHp * (move.power || 50) / 100);
        let oldHp = attacker.currentHp;
        attacker.currentHp = Math.min(attacker.maxHp, attacker.currentHp + healAmount);
        let actualHeal = attacker.currentHp - oldHp;
        addLog(`${attacker.name} recovered ${actualHeal} HP! (${attacker.currentHp}/${attacker.maxHp})`);
    }
}

function initBattleUI() { showScreen('battle-screen'); document.getElementById('battle-log').innerHTML = ''; renderBattle(); }

function renderBattle() {
    const p = getActivePokemon();
    const e = battleState.enemyPoke;
    let enemyStatus = e.status !== "None" ? `<br>Status: <span style="color:#e63946">${e.status}</span>` : '';
    document.getElementById('enemy-battle-box').innerHTML = `<strong>${e.name}</strong> (Lv.${e.level})<br>Types: ${e.types.join('/')}<br>HP: ${e.currentHp} / ${e.maxHp}${enemyStatus}`;
    let playerStatus = p.status !== "None" ? `<br>Status: <span style="color:#e63946">${p.status}</span>` : '';
    let playerStages = Object.entries(p.stages).filter(([k, v]) => v !== 0).map(([k, v]) => `${k}:${v > 0 ? '+' : ''}${v}`).join(', ');
    if (playerStages) playerStatus += `<br><small>Stages: ${playerStages}</small>`;

    let teamBar = '<div style="display:flex;gap:8px;margin-top:10px;justify-content:center;flex-wrap:wrap;">';
    battleState.playerTeam.forEach((poke, idx) => {
        let isActive = idx === battleState.activeIndex;
        let isFainted = poke.currentHp <= 0;
        let border = isActive ? '#457b9d' : isFainted ? '#333' : '#444';
        let bg = isActive ? '#1a2a33' : '#111';
        let opacity = isFainted ? '0.4' : '1';
        let hpPercent = Math.max(0, Math.round((poke.currentHp / poke.maxHp) * 100));
        let hpColor = hpPercent > 50 ? '#2a9d8f' : hpPercent > 20 ? '#e9c46a' : '#e63946';
        teamBar += `<div style="background:${bg};border:2px solid ${border};border-radius:6px;padding:8px 12px;opacity:${opacity};cursor:pointer;min-width:80px;text-align:center;" onclick="showSwitchMenu(${idx})">
            <div style="font-size:12px;font-weight:bold;">${poke.name}</div>
            <div style="font-size:10px;color:#888;">Lv.${poke.level}</div>
            <div style="width:60px;height:6px;background:#222;border-radius:3px;margin:4px auto;"><div style="width:${hpPercent}%;height:100%;background:${hpColor};border-radius:3px;"></div></div>
            <div style="font-size:10px;">${poke.currentHp}/${poke.maxHp}</div>
        </div>`;
    });
    teamBar += '</div>';

    document.getElementById('player-battle-box').innerHTML = `<strong>${p.name}</strong> (Lv.${p.level})<br>HP: ${p.currentHp} / ${p.maxHp}${playerStatus}${teamBar}`;

    let actionsHtml = '';
    if (battleState.isWild) actionsHtml += `<button class="action-btn" onclick="attemptCatch()">Catch</button> `;
    actionsHtml += `<button class="action-btn" onclick="showSwitchMenu()">Switch</button> `;
    actionsHtml += `<button class="action-btn" onclick="runAway()">Run</button>`;
    const movesHtml = p.moves.map(m => `<button class="action-btn" onclick="executeTurn(${m.id})">${m.name}<br><small>${m.type} | ${m.category} | Pwr:${m.power}</small></button>`).join('');
    document.getElementById('battle-actions').innerHTML = movesHtml + '<br>' + actionsHtml;
}

function addLog(msg) { const log = document.getElementById('battle-log'); log.innerHTML += msg + '<br>'; log.scrollTop = log.scrollHeight; }

function showSwitchMenu(forceIndex = null) {
    if (forceIndex !== null && forceIndex === battleState.activeIndex) return;
    let aliveTeam = battleState.playerTeam.map((p, idx) => ({ ...p, idx })).filter(p => p.currentHp > 0 && p.idx !== battleState.activeIndex);
    if (aliveTeam.length === 0) { alert("No other Pokemon available!"); return; }
    let html = `<div class="box" style="max-width:400px;max-height:80vh;overflow-y:auto;text-align:left;"><h3 style="text-align:center;margin-bottom:10px;">Switch Pokemon</h3>`;
    aliveTeam.forEach(p => {
        let hpPercent = Math.max(0, (p.currentHp / p.maxHp) * 100);
        let hpColor = hpPercent > 50 ? '#2a9d8f' : hpPercent > 20 ? '#e9c46a' : '#e63946';
        let hasMoves = p.moves[0].name !== "Struggle";
        html += `<div class="list-item" style="cursor:pointer;margin-bottom:8px;" onclick="executeSwitch(${p.idx})">
            <strong>${p.name}</strong> (Lv.${p.level}) - ${p.types.join('/')}<br>
            HP: <span style="color:${hpColor}">${p.currentHp}/${p.maxHp}</span> | Status: ${p.status}
            ${!hasMoves ? '<br><span style="color:#e63946;">No moves set - will use Struggle</span>' : ''}
        </div>`;
    });
    html += `<button class="action-btn" style="width:100%;margin-top:10px;" onclick="hideSwitchMenu()">Cancel</button></div>`;
    const overlay = document.getElementById('switch-overlay');
    overlay.innerHTML = html;
    overlay.style.display = 'flex';
    overlay.style.position = 'fixed';
    overlay.style.top = '0';
    overlay.style.left = '0';
    overlay.style.width = '100%';
    overlay.style.height = '100%';
    overlay.style.background = 'rgba(0,0,0,0.85)';
    overlay.style.zIndex = '1000';
    overlay.style.justifyContent = 'center';
    overlay.style.alignItems = 'center';
}

function hideSwitchMenu() {
    const overlay = document.getElementById('switch-overlay');
    overlay.style.display = 'none';
    overlay.innerHTML = '';
}

async function executeSwitch(newIndex) {
    hideSwitchMenu();
    let oldPokemon = getActivePokemon();
    battleState.activeIndex = newIndex;
    let newPokemon = getActivePokemon();
    addLog(`${oldPokemon.name}, come back!`);
    addLog(`Go, ${newPokemon.name}!`);
    renderBattle();
    if (!battleState.switchingAfterFaint) {
        let eMove = battleState.enemyPoke.moves.length > 0 ? battleState.enemyPoke.moves[Math.floor(Math.random() * battleState.enemyPoke.moves.length)] : null;
        if (eMove) { await executeMoveTurn(battleState.enemyPoke, newPokemon, eMove); renderBattle(); checkBattleEnd(); }
    } else { battleState.switchingAfterFaint = false; }
}

function handlePlayerFaint() {
    let aliveTeam = getAliveTeam();
    if (aliveTeam.length === 0) { return true; }
    battleState.switchingAfterFaint = true;
    addLog(`${getActivePokemon().name} fainted!`);
    showSwitchMenu();
    return false;
}

async function executeTurn(moveId) {
    const p = getActivePokemon();
    const pMove = p.moves.find(m => m.id === moveId);
    const eMove = battleState.enemyPoke.moves.length > 0 ? battleState.enemyPoke.moves[Math.floor(Math.random() * battleState.enemyPoke.moves.length)] : null;
    const playerSpeed = getEffectiveStat(p, "Speed");
    const enemySpeed = getEffectiveStat(battleState.enemyPoke, "Speed");
    const playerFirst = playerSpeed >= enemySpeed;

    if (playerFirst) {
        await executeMoveTurn(p, battleState.enemyPoke, pMove);
        renderBattle();
        if (battleState.enemyPoke.currentHp <= 0) {
            checkBattleEnd();
            return;
        }
        if (p.currentHp <= 0) {
            if (handlePlayerFaint()) { playerLoses(); }
            return;
        }
        if (eMove) {
            await executeMoveTurn(battleState.enemyPoke, getActivePokemon(), eMove);
            renderBattle();
            if (battleState.enemyPoke.currentHp <= 0) {
                checkBattleEnd();
                return;
            }
            if (getActivePokemon().currentHp <= 0) {
                if (handlePlayerFaint()) { playerLoses(); }
                return;
            }
        }
    } else {
        await executeMoveTurn(battleState.enemyPoke, p, eMove);
        renderBattle();
        if (battleState.enemyPoke.currentHp <= 0) {
            checkBattleEnd();
            return;
        }
        if (p.currentHp <= 0) {
            if (handlePlayerFaint()) { playerLoses(); }
            return;
        }
        await executeMoveTurn(getActivePokemon(), battleState.enemyPoke, pMove);
        renderBattle();
        if (battleState.enemyPoke.currentHp <= 0) {
            checkBattleEnd();
            return;
        }
        if (getActivePokemon().currentHp <= 0) {
            if (handlePlayerFaint()) { playerLoses(); }
            return;
        }
    }

    checkBattleEnd();
}

async function executeMoveTurn(attacker, defender, move) {
    let statusResult = applyStartOfTurnEffects(attacker);
    if (statusResult === "fainted") { renderBattle(); return "fainted"; }
    if (statusResult === "cant_move") { renderBattle(); return "skipped"; }
    if (attacker.status === "Paralyze" && Math.random() < 0.25) { addLog(`${attacker.name} is paralyzed and can't move!`); return "skipped"; }
    if (!move) { addLog(`${attacker.name} has no moves!`); return "skipped"; }
    if (move.category === "Status") { applyMoveEffect(attacker, defender, move); return "ok"; }
    await doAttack(attacker, defender, move);
    return "ok";
}

async function doAttack(attacker, defender, move) {
    addLog(`${attacker.name} used ${move.name}!`);
    if (move.accuracy < 100 && Math.random() * 100 >= move.accuracy) { addLog(`${attacker.name}'s attack missed!`); return; }
    try {
        const atkStat = move.category === "Physical" ? attacker.baseAtk : attacker.baseSpA;
        const defStat = move.category === "Physical" ? defender.baseDef : defender.baseSpD;
        const atkStage = move.category === "Physical" ? attacker.stages.Attack : attacker.stages.SpecialAttack;
        const defStage = move.category === "Physical" ? defender.stages.Defense : defender.stages.SpecialDefense;
        const res = await req('POST', '/battle/damage', {
            attackerLevel: attacker.level, attackerAttackStat: atkStat, defenderDefenseStat: defStat,
            movePower: move.power, moveType: move.type, moveCategory: move.category,
            attackerPrimaryType: attacker.types[0], attackerSecondaryType: attacker.types[1] || null,
            defenderPrimaryType: defender.types[0], defenderSecondaryType: defender.types[1] || null,
            attackerStage: atkStage, defenderStage: defStage,
            attackerStatus: attacker.status, attackerHasGuts: false
        });
        defender.currentHp = Math.max(0, defender.currentHp - res.damage);
        let effText = res.effectiveness > 1 ? "Super effective!" : res.effectiveness > 0 && res.effectiveness < 1 ? "Not very effective..." : res.effectiveness === 0 ? "It had no effect!" : "";
        addLog(`Dealt ${res.damage} damage! ${effText}`);
        if (move.secondaryEffectChance && move.applyStatus && move.applyStatus !== "None") {
            if (Math.random() * 100 < move.secondaryEffectChance) {
                if (defender.status === "None") {
                    let canApply = true;
                    if (move.applyStatus === "Burn" && defender.types.includes("Fire")) canApply = false;
                    if ((move.applyStatus === "Poison" || move.applyStatus === "Toxic") && (defender.types.includes("Poison") || defender.types.includes("Steel"))) canApply = false;
                    if (move.applyStatus === "Paralyze" && defender.types.includes("Electric")) canApply = false;
                    if (canApply) {
                        defender.status = move.applyStatus;
                        defender.statusTurns = 0;
                        if (move.applyStatus === "Sleep") { defender.statusTurns = Math.floor(Math.random() * 3) + 1; }
                        addLog(`${defender.name} is now ${move.applyStatus.toLowerCase()}!`);
                    }
                }
            }
        }
        if (defender.status === "Freeze" && move.type === "Fire") { defender.status = "None"; defender.statusTurns = 0; addLog(`${defender.name} was thawed out by the fire!`); }
    } catch (e) { addLog("Error calculating damage."); }
}

async function attemptCatch() {
    try {
        const e = battleState.enemyPoke;
        const res = await req('POST', '/battle/catch', { speciesId: e.id, level: e.level, currentHp: e.currentHp, maxHp: e.maxHp, gender: 1 });
        addLog(res.message);
        if (res.success) {
            logBattle(true);
            alert(res.message);
            showScreen('menu-screen');
        }
        else {
            await executeMoveTurn(battleState.enemyPoke, getActivePokemon(), { id: 999, name: "Struggle", type: "Normal", category: "Physical", power: 50, accuracy: 100, pp: 10, effectType: "None" });
            renderBattle();
            if (getActivePokemon().currentHp <= 0) { handlePlayerFaint(); }
        }
    } catch (e) { alert(e.message); }
}

function logBattle(isVictory) {
    const logDiv = document.getElementById('battle-log');
    const turns = logDiv.innerHTML
        .split('<br>')
        .filter(line => line.trim().length > 0);

    req('POST', '/battle/log', {
        isVictory: isVictory,
        turns: turns
    }).catch(e => {
        console.error("Failed to log battle:", e);
    });
}

function playerLoses() {
    logBattle(false);
    addLog("All your Pokemon fainted!");
    alert("You lost!");
    showScreen('menu-screen');
}

function checkBattleEnd() {
    if (getAliveTeam().length === 0) {
        playerLoses();
        return;
    }

    if (battleState.enemyPoke.currentHp <= 0) {
        addLog(`Enemy ${battleState.enemyPoke.name} fainted!`);

        if (battleState.isWild) {
            logBattle(true);
            req('POST', '/battle/victory').then(() => {
                alert("Victory! Team leveled up!");
                showScreen('menu-screen');
            });
        } else {
            battleState.currentNpcIdx++;
            if (battleState.currentNpcIdx < battleState.npcTeam.length) {
                battleState.enemyPoke = mapNpcToBattlePoke(battleState.npcTeam[battleState.currentNpcIdx]);
                addLog(`Go, ${battleState.enemyPoke.name}!`);
                renderBattle();
            } else {
                logBattle(true);
                req('POST', '/battle/victory').then(() => {
                    alert("You beat the trainer! Team leveled up!");
                    showScreen('menu-screen');
                });
            }
        }
    }
}

function runAway() { if (confirm("Run away?")) showScreen('menu-screen'); }