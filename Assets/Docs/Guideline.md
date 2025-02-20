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
### Open/Closed Principle (OCP)
### Liskov Substitution Principle (LSP)
### Interface Segregation Principle (ISP)
### Dependency Inversion Principle (DIP)
### KISS Principle (Keep It Simple, Stupid)
### DRY Principle (Don't Repeat Yourself)
### YAGNI Principle (You Aren't Gonna Need It)

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

    #endif
}