// ============================================================================
// Camada de interface (UI). NÃO contém regras de negócio: apenas consome a
// Web API, que por sua vez delega ao domínio. Isso mantém as camadas separadas.
// ============================================================================

let token = null;
let timerAquecimento = null;
let alvoTeclado = 'tempo'; // 'tempo' | 'potencia'

const $ = (id) => document.getElementById(id);

// ---------------------------------------------------------------- API helper
async function api(caminho, metodo = 'GET', corpo = null) {
  const opcoes = { method: metodo, headers: {} };
  if (token) opcoes.headers['Authorization'] = 'Bearer ' + token;
  if (corpo !== null) {
    opcoes.headers['Content-Type'] = 'application/json';
    opcoes.body = JSON.stringify(corpo);
  }
  const resp = await fetch('/api/' + caminho, opcoes);
  let dados = null;
  try { dados = await resp.json(); } catch { /* sem corpo */ }
  if (!resp.ok) {
    const msg = (dados && (dados.mensagem || dados.Mensagem)) || 'Erro na requisição.';
    throw new Error(msg);
  }
  return dados;
}

function mostrarMensagem(texto, tipo = 'ok') {
  const el = $('mensagem');
  el.textContent = texto;
  el.className = 'mensagem visivel ' + tipo;
  setTimeout(() => { el.className = 'mensagem'; }, 3200);
}

// ---------------------------------------------------------------- Autenticação
async function login() {
  try {
    const r = await api('autenticacao/login', 'POST', {
      usuario: $('loginUsuario').value, senha: $('loginSenha').value
    });
    token = r.token;
    $('statusAutenticacao').textContent = 'Autenticado';
    $('statusAutenticacao').className = 'badge badge-on';
    $('painelPrincipal').classList.remove('desabilitado');
    mostrarMensagem('Autenticado com sucesso.', 'ok');
    await carregarProgramas();
    await atualizarStatus();
  } catch (e) {
    mostrarMensagem(e.message, 'erro');
  }
}

async function configurar() {
  try {
    await api('autenticacao/configurar', 'POST', {
      usuario: $('cfgUsuario').value, senha: $('cfgSenha').value
    });
    mostrarMensagem('Credenciais salvas. Faça login com os novos dados.', 'ok');
  } catch (e) {
    mostrarMensagem(e.message, 'erro');
  }
}

// ---------------------------------------------------------------- Teclado digital
function configurarTeclado() {
  document.querySelectorAll('.tecla').forEach((b) => {
    b.addEventListener('click', () => {
      const num = b.getAttribute('data-num');
      const acao = b.getAttribute('data-acao');
      const campo = alvoTeclado === 'tempo' ? $('campoTempo') : $('campoPotencia');
      if (num !== null) {
        campo.value = (campo.value || '') + num;
      } else if (acao === 'limpar') {
        $('campoTempo').value = '';
        $('campoPotencia').value = '';
      } else if (acao === 'alvo') {
        alvoTeclado = alvoTeclado === 'tempo' ? 'potencia' : 'tempo';
        $('alvoTeclado').innerHTML = 'Teclado digitando em: <strong>' +
          (alvoTeclado === 'tempo' ? 'Tempo' : 'Potência') + '</strong> (⇄ alterna)';
      }
    });
  });
}

// ---------------------------------------------------------------- Aquecimento
async function iniciar() {
  try {
    const tempoTxt = $('campoTempo').value.trim();
    const potTxt = $('campoPotencia').value.trim();
    const corpo = {
      tempoSegundos: tempoTxt === '' ? null : parseInt(tempoTxt, 10),
      potencia: potTxt === '' ? null : parseInt(potTxt, 10)
    };
    const status = await api('aquecimento/iniciar', 'POST', corpo);
    aplicarStatus(status);
    garantirTimer(status);
  } catch (e) {
    mostrarMensagem(e.message, 'erro');
  }
}

async function pausarCancelar() {
  try {
    const status = await api('aquecimento/pausar-cancelar', 'POST', {});
    aplicarStatus(status);
    if (status.estado !== 'EmAndamento') pararTimer();
    // Se cancelou/limpou, limpa também os campos de entrada.
    if (status.estado === 'Parado') { $('campoTempo').value = ''; $('campoPotencia').value = ''; }
  } catch (e) {
    mostrarMensagem(e.message, 'erro');
  }
}

async function iniciarPrograma(nome) {
  try {
    const status = await api('aquecimento/programa/' + encodeURIComponent(nome), 'POST', {});
    aplicarStatus(status);
    garantirTimer(status);
  } catch (e) {
    mostrarMensagem(e.message, 'erro');
  }
}

function garantirTimer(status) {
  if (status.estado === 'EmAndamento' && !timerAquecimento) {
    timerAquecimento = setInterval(avancar, 1000);
  }
}
function pararTimer() {
  if (timerAquecimento) { clearInterval(timerAquecimento); timerAquecimento = null; }
}

async function avancar() {
  try {
    const status = await api('aquecimento/avancar', 'POST', {});
    aplicarStatus(status);
    if (status.estado !== 'EmAndamento') {
      pararTimer();
      if (status.concluido) mostrarMensagem('Aquecimento concluído!', 'ok');
    }
  } catch (e) {
    pararTimer();
    mostrarMensagem(e.message, 'erro');
  }
}

async function atualizarStatus() {
  try {
    const status = await api('aquecimento/status', 'GET');
    aplicarStatus(status);
    garantirTimer(status);
  } catch { /* ignora */ }
}

function aplicarStatus(s) {
  $('visorTempo').textContent = s.tempoFormatado || '0:00';
  $('visorPotencia').textContent = 'Potência: ' + (s.potencia > 0 ? s.potencia : '-');
  $('visorPrograma').textContent = s.programaSelecionado ? ('Programa: ' + s.programaSelecionado) : '';
  $('visorProgresso').textContent = s.textoProgresso || '';
}

// ---------------------------------------------------------------- Programas
async function carregarProgramas() {
  try {
    const programas = await api('programas', 'GET');
    const container = $('listaProgramas');
    container.innerHTML = '';
    programas.forEach((p) => {
      const div = document.createElement('div');
      div.className = 'item-programa' + (p.customizado ? ' customizado' : '');
      const min = Math.floor(p.tempoSegundos / 60), seg = p.tempoSegundos % 60;
      div.innerHTML =
        '<div><div class="nome">' + escaparHtml(p.nome) + '</div>' +
        '<div class="detalhe">' + escaparHtml(p.alimento) +
        ' — ' + min + ':' + String(seg).padStart(2, '0') +
        ' — pot. ' + p.potencia + ' — "' + escaparHtml(p.caractere) + '"</div></div>' +
        '<div class="acoes"></div>';
      const acoes = div.querySelector('.acoes');
      const btn = document.createElement('button');
      btn.textContent = 'Iniciar';
      btn.className = 'primario';
      btn.onclick = () => iniciarPrograma(p.nome);
      acoes.appendChild(btn);
      if (p.customizado) {
        const del = document.createElement('button');
        del.textContent = 'Excluir';
        del.onclick = () => removerPrograma(p.nome);
        acoes.appendChild(del);
      }
      container.appendChild(div);
    });
  } catch (e) {
    mostrarMensagem(e.message, 'erro');
  }
}

async function cadastrarPrograma() {
  try {
    const corpo = {
      nome: $('npNome').value,
      alimento: $('npAlimento').value,
      tempoSegundos: parseInt($('npTempo').value, 10) || 0,
      potencia: parseInt($('npPotencia').value, 10) || 0,
      caractere: $('npCaractere').value,
      instrucoes: $('npInstrucoes').value
    };
    await api('programas', 'POST', corpo);
    mostrarMensagem('Programa cadastrado!', 'ok');
    ['npNome','npAlimento','npTempo','npPotencia','npCaractere','npInstrucoes']
      .forEach((id) => $(id).value = '');
    await carregarProgramas();
  } catch (e) {
    mostrarMensagem(e.message, 'erro');
  }
}

async function removerPrograma(nome) {
  try {
    await api('programas/' + encodeURIComponent(nome), 'DELETE');
    mostrarMensagem('Programa removido.', 'ok');
    await carregarProgramas();
  } catch (e) {
    mostrarMensagem(e.message, 'erro');
  }
}

function escaparHtml(txt) {
  const d = document.createElement('div');
  d.textContent = txt == null ? '' : String(txt);
  return d.innerHTML;
}

// ---------------------------------------------------------------- Bootstrap
window.addEventListener('DOMContentLoaded', () => {
  $('btnLogin').onclick = login;
  $('btnConfigurar').onclick = configurar;
  $('btnIniciar').onclick = iniciar;
  $('btnPausar').onclick = pausarCancelar;
  $('btnCadastrar').onclick = cadastrarPrograma;
  configurarTeclado();
});
