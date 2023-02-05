namespace Dissonance.Engine;

// Initialization

[CallbackDeclaration]
public delegate void EngineInitialization();

[CallbackDeclaration]
public delegate void GameInitialization();

// Disposing

[CallbackDeclaration]
public delegate void EngineTermination();

[CallbackDeclaration]
public delegate void GameTermination();

// Loop

[CallbackDeclaration]
public delegate void FixedUpdate();

[CallbackDeclaration]
public delegate void RenderUpdate();
