# Cliente Mobile MMORPG - Unity

Cliente Android para o MMORPG mobile, desenvolvido em Unity com interface otimizada para toque.

## Requisitos

- Unity 2022 LTS ou superior
- Android SDK 26+ (API Level 26+)
- Visual Studio ou JetBrains Rider para edição de scripts

## Estrutura de Pastas

```
Assets/
├── Scripts/
│   ├── Config.cs                 - Configurações globais
│   ├── NetworkClient.cs          - Cliente TCP
│   ├── AuthManager.cs            - Autenticação
│   ├── CharacterManager.cs       - Gerenciamento de personagem
│   ├── PlayerController.cs       - Controle do jogador
│   ├── VirtualJoystick.cs        - Joystick virtual
│   ├── LoginUI.cs                - UI de login
│   ├── CharacterCreationUI.cs    - UI de criação
│   ├── GameHUD.cs                - HUD principal
│   └── ...
├── Prefabs/                      - Prefabs reutilizáveis
├── Scenes/                       - Cenas do jogo
├── Models/                       - Modelos 3D
├── Textures/                     - Texturas
├── Audio/                        - Áudio (música e SFX)
└── UI/                           - Elementos de UI
```

## Cenas Principais

1. **Login**: Tela de login/registro
2. **CharacterCreation**: Criação de novo personagem
3. **MainGame**: Cena principal do jogo
4. **Map**: Mapa 3D navegável

## Configuração

### 1. Conectar ao Servidor

Edite `Config.cs`:
```csharp
public static string ServerIP = "seu-servidor.com";
public static int ServerPort = 8080;
```

### 2. Build para Android

1. File → Build Settings
2. Selecione "Android" como plataforma
3. Configure as configurações do Android:
   - Minimum API Level: 26
   - Target API Level: 33
   - Screen Orientation: Portrait
4. Clique em "Build and Run"

### 3. Assinatura do APK

Para distribuição:
1. Gere uma chave de assinatura
2. Configure em Player Settings → Publishing Settings
3. Build o APK assinado

## Sistemas Implementados

### ✓ Autenticação
- Login com username e senha
- Registro de novo usuário
- Armazenamento seguro de token JWT

### ✓ Gerenciamento de Personagem
- Criação com 6 classes diferentes
- Carregamento de dados
- Atualização de posição em tempo real

### ✓ Controles Mobile
- Joystick virtual analógico
- Botões de ação adaptados
- Gestos de toque

### ✓ Interface
- HUD com status do personagem
- Painéis de inventário, quests, stats
- Menu de configurações

### ✓ Rede
- Comunicação TCP com servidor
- Serialização JSON
- Reconexão automática

## Scripts Principais

### NetworkClient
Gerencia conexão TCP com servidor. Singleton que persiste entre cenas.

```csharp
NetworkClient.Instance.Connect(host, port);
NetworkClient.Instance.SendMessage(json);
```

### AuthManager
Gerencia autenticação de usuário.

```csharp
AuthManager.Instance.Login(username, password);
AuthManager.Instance.Register(username, email, password);
```

### CharacterManager
Gerencia dados do personagem.

```csharp
CharacterManager.Instance.CreateCharacter(name, class);
CharacterManager.Instance.LoadCharacter(characterId);
```

### PlayerController
Controla movimento do personagem no mapa.

```csharp
// Conecta ao joystick virtual
_joystick.OnJoystickMove.AddListener(OnJoystickMove);
```

## Desenvolvimento

### Adicionar Nova Cena

1. Create → Scene
2. Adicione os managers necessários (NetworkClient, AuthManager, etc.)
3. Configure a UI
4. Registre em Build Settings

### Adicionar Novo Sistema

1. Crie um novo script em `Assets/Scripts/`
2. Implemente como Singleton se necessário
3. Use UnityEvents para comunicação entre sistemas
4. Adicione testes

## Build e Deploy

### APK Debug
```bash
# No Unity
File → Build Settings → Build (gera APK debug)
```

### APK Release
```bash
# Configurar assinatura em Player Settings
# File → Build Settings → Build
```

### Instalação em Dispositivo
```bash
adb install -r app.apk
```

## Troubleshooting

**Erro de conexão ao servidor:**
- Verifique se o servidor está rodando
- Confirme IP e porta em Config.cs
- Verifique firewall

**Erro ao fazer login:**
- Verifique credenciais
- Verifique se usuário existe no banco
- Verifique logs do servidor

**Erro ao criar personagem:**
- Verifique se nome é único
- Verifique classe selecionada
- Verifique espaço em disco

## Otimizações Mobile

- Texturas comprimidas (ASTC, ETC2)
- LOD (Level of Detail) para modelos
- Object pooling para efeitos
- Culling de câmera para performance
- Async loading de assets

## Próximas Funcionalidades

- [ ] Sistema de combate completo
- [ ] Quests e missões
- [ ] Inventário expandido
- [ ] Batalhas navais
- [ ] PvP em arenas
- [ ] Clã e guildas
- [ ] Chat multiplayer
- [ ] Notificações push
