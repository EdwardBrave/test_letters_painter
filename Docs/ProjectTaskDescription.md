# Project Task Description

## **Initial Information:**

The product is an animated mobile 2D iOS application for preschool children for the US market. The application is developed in C# using Unity. Inside the app, there are cartoons + mini-games that introduce the child to the outside world, teach letters, numbers, etc. For example, a game that teaches a child how to write letters.

For the test task, we propose making a mini-game. The test task is submitted as a Unity project ready for building. Acceptable: a link to a git repository (preferred) or a project archive without service folders (`Library`, `Temp`, `obj`, `Logs`, `Build`). It must be possible not only to view the code but also to build and test the application on a device/in the editor.

## **General:**

- Unity version: `6000.0.74f1`
- The game must adapt correctly across aspect ratios ranging from 19.5:9 (iPhone 15 Pro, 2556×1179) to 4:3 (iPad Pro 12.9", 2732×2048).
- Test devices: iPhone 15 Pro and iPad Pro 12.9".
- The application must be locked to landscape orientation.
- The frame rate must be 60 FPS.
- The interaction format between the player and the application is tap and swipe.

## **Technical Requirements:**

- Use **Zenject (Extenject)** for dependency management.
- Level icons in the Menu and assets for tracing in the Game must be loaded via **Addressables**.
- Implement asynchronous scenarios using **UniTask**.
- Level data must be externalized into a **JSON** configuration.
- The Menu screen must be implemented using an MVx approach (MVC / MVP / MVVM).
- During normal gameplay, the console must remain clean—free of errors and unhandled exceptions (NullReferenceException, etc.).
- The tracing engine must be completely **data-driven**—one universal code for any glyph, without branching for specific letters/digits/shapes (details in the "Tracing Routes" section).

## Tests:

The project must contain an automated configuration validity test (EditMode, no play mode required), runnable via the Unity Test Runner.
The test loads all JSON level configs and verifies their integrity. It should fail with an informative message if:

- The JSON cannot be deserialized or does not match the expected schema;
- A level refers to a non-existent asset (Addressables key does not resolve);
- A route is empty: the shape has no segments, or a segment has fewer than 2 points;
- There are duplicate level IDs.

## **Assets:**

All visual and audio assets are provided in `Assets/Content` folder: level icons, silhouettes of letters/digits/shapes, tracing route elements, a mascot character with a spaceship, a Home button, and voice phrases.

<aside>
⚠️ The archive also contains a References folder. It is expected that the UI will be assembled according to the screenshots in the References folder.
</aside>

## **Tasks:**

### **"Menu" Scene**

In the menu, the player selects a level for the upcoming game. Level icons are loaded at the start of the scene (initially, the scene consists only of a background that you assemble from primitives). Clicking an icon loads the Game scene with the selected letter/digit/shape for tracing.

The screen consists of categories. Categories are scrolled vertically, and levels within a category are scrolled horizontally. There are three categories (according to the screenshots below): Trace letters, Trace numbers, Trace shapes (shapes - circle).

Each category contains 7 levels. These are 7 color variations of the same letter/digit/shape according to the provided assets—the 7 colors of the rainbow (red → violet), as shown in the All Levels screenshot.

![Levels.png](Levels.png)

*Screenshot: All Levels*

All headings and texts ("Games", "Trace letters", "Trace numbers", "Trace shapes"), as well as fonts, colors, sizes, and layouts, must match the screenshots. The layout for iPhone should follow the Menu (iPhone) screenshot, and for iPad, it should follow the Menu (iPad) screenshot: more categories and cards are visible at once on a wider screen.

![Menu_Iphone.png](Menu_Iphone.png)

*Screenshot: Menu (iPhone)*

![Menu_Ipad.png](Menu_Ipad.png)

*Screenshot: Menu (iPad) - the third category, Trace shapes, is visible*

### **"Game" Scene**

In the center of the scene is an unfilled silhouette of a letter/digit/shape. The tracing shape itself is loaded at the start of the scene (initially, the scene consists only of the background). The silhouette is semi-transparent—a barely noticeable outline/hint (reference - *Game* screenshot).

At the start of the level, a voicephrase is played ("Follow the stars and trace the letter" for letters, "Follow the stars and trace the number" for digits and the circle). After the phrase ends, the tracing route appears: a chain of circles with a star at the end of the current stroke (example in the screenshots). The route shapes appear sequentially and smoothly from 0% opacity over 1 second.

At the beginning of the current stroke, the mascot character appears in a spaceship (provided asset)—this is the "cursor" that the player guides with their finger. The player swipes to lead the mascot along the circles to the star, following the route inside the shape. A colored trail is drawn behind the spaceship, representing the coloring of the shape. The color of the trail corresponds to the color of the current level (retrieved from the level configuration) rather than being statically defined.

**Tracing Logic.**

Drawing occurs only when the finger is within a corridor around the route line. The width of the corridor is a configurable parameter (configured in the config/inspector, not hardcoded); the specific value should be adjusted for the child's comfort. Movement is registered only forward along the specified direction: backward movement does not paint and does not roll back progress.

If the finger leaves the corridor or moves backward, painting is paused, the trail remains at the reached point, and the stroke progress is not reset. When the finger returns to the corridor no further than the last reached point, tracing resumes from the point of pause. Lifting the finger from the screen also only pauses the stroke. The stroke is complete when the trail is guided to the star at the end of the route.

When one stroke is guided to the star, the next stroke appears in a similar fashion (with its mascot, circles, and star). When all strokes of the shape are completed, the level is considered finished: one of the phrases "Awesome", "Excellent", or "Thats_good" is played randomly, and a transition to the next level occurs.

If the player completes all levels of a section, they repeat in a loop. To exit to the menu, the player presses the Home button in the upper-left corner of the scene (see screenshot).

![Game_Iphone.png](Game_Iphone.png)

*Screenshot: Game (iPhone) - mascot in the ship draws a trail along the route; Home button in the upper left*

![Game_IPad.png](Game_IPad.png)

*Screenshot: Game (iPad)*

### Tracing Routes

The number and direction of strokes for each shape are shown in the diagram below (numbers = stroke order, arrows = direction of movement):

![Tracing routes.png](Tracing_routes.png)

*Route diagram: A - 3 strokes, 1 - 2 strokes, O (circle) - 1 stroke*

<aside>
⚠️ The method of defining the geometry is at the candidate's discretion (positioning points in the config, custom editor, etc.). Landmarks: the Game screenshot shows how the route looks to the player (points + star), and the A / 1 / O diagrams specify the number of segments, their order, and their direction.
</aside>

<aside>
⚠️ The tracing logic must be entirely data-driven and independent of the specific glyph.
The glyph is defined as an ordered set of strokes, and each stroke is defined as a path (e.g., a polyline or a curve) with a specified direction. The engine must execute the route without any special-case code for a specific letter/digit/shape. The route visual elements (points, star, trail) are generated based on the route data.

Adding a new glyph should only require a new entry in the config and the corresponding asset, without any changes to the code or scenes.
</aside>

## **Hints:**

An inactivity timer counts from the player's last action (any tap/swipe resets it to zero). Both hints are based on this timer:

- In case of inactivity for more than 7 seconds, the task instruction phrase played at the beginning of the scene is repeated.
- In case of inactivity for more than 14 seconds, a finger appears and starts moving along the route cyclically at an arbitrary speed (example in the screenshot below). When the player starts drawing, the hint disappears.

![Helper.png](Helper.png)

*Screenshot: Game (iPhone) - finger helper moves along the route*
