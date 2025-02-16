## Code Style and Conventions
- Use PascalCase for public members, camelCase for private members.
- Utilize #regions to organize code sections.
- Wrap editor-only code with #if UNITY_EDITOR.
- Use [SerializeField] to expose private fields in the inspector.
- Group data in serializable classes or structs to clean up the inspector.
- Implement Range attributes for float fields when appropriate.
- Variable names reveal intent Make names searchable and pronounceable
- Avoid single letter abbreviations unless a counter or expression
- Use nouns. Reserve verbs for methods unless it's a bool
- Beeleans ask a question that can be answered true or false
- Avoid redundant names: If your class is called `Player`, you don't need to create member variables called `PlayerScore` or `PlayerTarger`. Trim them down to Score or Target.
- Use the var keyword for implicitly tryped local variables if it helps readability and the type is obvious.
- A properly named class, variable, or method serves in place of a comment.
- Use a tooltip instead of a commnent for serialized fields.
- Do not introduce namespace until necessary.

### Events and event handlers
- Name the event with a verb phrase: Choose a name that communicates the state change accurately. Use the present or past participle to indicate events "before" or "after". For example, specify "OpeningDoor" for an event before opening a door or "DoorOpened" for an event afterware.
- Prefix the event raising method (in the subject) with "On": The subject that invokes the event typically does so from a method prefixed with "On," e.g. "OnOpeningDoor" or "OnDoorOpened".
- Prefix the event handling method (in the observer) with the subject's name and underscore(_): If the subject is named "GameEvents", your observer can have a method called "GameEvents_OpeningDoor" or "GameEvents_DoorOpened". Note that this is called the "event handling method", not to be confused with the EventHandler delegate.

## Best Practices
- Use TryGetComponent to avoid null reference exceptions.
- Prefer direct references or GetComponent() over GameObject.Find() or Transform.Find().
- Always use TextMeshPro for text rendering.
- Separate View and Logic.
- Prefer composition over inheritance.

### Proper Use of Interfaces and Delegates
- Interfaces: Use interfaces when different classes need to perform the same operation.
- Delegates: Use delegates when the same class needs to perform the same operation in different ways.

## Stick to Design Principle
### Single Responsibility Principle (SRP)
- Core Idea: A class should have only one reason to change, meaning it should have only one responsibility.
- Significance: Reduces class complexity, improving maintainability and testability.
- Example: A character class should not handle rendering, AI logic, and saving logic. These responsibilities should be separated into different components.
### Open/Closed Principle (OCP)
- Core Idea: Software entities (classes, modules, functions, etc.) should be open for extension but closed for modification.
- Significance: Enables extending functionality without modifying existing code, reducing the risk of introducing new issues.
- Example: Use interfaces or abstract classes to allow adding new character types without altering existing code.
### Liskov Substitution Principle (LSP)
- Core Idea: Subclasses must be replaceable with their base classes without affecting the correctness of the program.
- Significance: Ensures the correctness of inheritance hierarchies and avoids unexpected behavior when substituting base classes with derived ones.
- Example: If a function accepts a base class parameter, it should work seamlessly with subclass instances.
### Interface Segregation Principle (ISP)
- Core Idea: A class should not be forced to implement interfaces it doesn't use. Large interfaces should be split into smaller, more specific ones.
- Significance: Prevents classes from taking on unrelated responsibilities, increasing modularity.
- Example: Instead of a single ICharacterActions interface, use smaller ones like IMovable (for movement) and IAttackable (for being attacked).
### Dependency Inversion Principle (DIP)
- Core Idea: High-level modules should not depend on low-level modules; both should depend on abstractions. Abstractions should not depend on details; details should depend on abstractions.
- Use Dependency Injection to manage dependencies.
- Significance: Reduces coupling between modules by relying on abstractions (e.g., interfaces or abstract classes).
- Example: A character class depends on a weapon interface, not a specific weapon implementation, enabling dynamic weapon swapping.
### KISS Principle (Keep It Simple, Stupid)
- Core Idea: Keep the code simple and straightforward, avoiding unnecessary complexity.
- Significance: Simpler code is easier to understand, debug, and maintain.
### DRY Principle (Don't Repeat Yourself)
- Core Idea: Avoid duplicating code; extract repetitive logic into common functions or modules.
- Significance: Reduces redundancy, increases reusability, and lowers the cost of changes.
### YAGNI Principle (You Aren't Gonna Need It)
- Core Idea: Do not implement functionality that is not currently required.
- Significance: Prevents over-engineering and allows focusing on current requirements.

## Nomenclature
- Variables: m_VariableName
- Constants: c_ConstantName
- Statics: s_StaticName
- Classes/Structs: ClassName
- Properties: PropertyName
- Methods: MethodName()
- Arguments: _argumentName
- Temporary variables: temporaryVariable

## Example Code Structure

public class ExampleClass : MonoBehaviour
{
    #region Constants
    private const int c_MaxItems = 100;
    #endregion

    #region Private Fields
    [SerializeField] private int m_ItemCount;
    [SerializeField, Range(0f, 1f)] private float m_SpawnChance;
    #endregion

    #region Public Properties
    public int ItemCount => m_ItemCount;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
    }

    private void Update()
    {
        UpdateGameLogic();
    }
    #endregion

    #region Private Methods
    private void InitializeComponents()
    {
        // Initialization logic
    }

    private void UpdateGameLogic()
    {
        // Update logic
    }
    #endregion

    #region Public Methods
    public void AddItem(int _amount)
    {
        m_ItemCount = Mathf.Min(m_ItemCount + _amount, c_MaxItems);
    }
    #endregion

    #if UNITY_EDITOR
    [ContextMenu("Debug Info")]
    private void DebugInfo()
    {
        Debug.Log($"Current item count: {m_ItemCount}");
    }
    #endif
}