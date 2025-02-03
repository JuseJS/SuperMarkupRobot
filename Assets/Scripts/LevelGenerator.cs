using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public int floorCount;
    public TagGroup[] tagGroups;
    public float floorHeight = 10f;
    public Vector2 floorDimensions = new Vector2(40f, 40f);

    [Serializable]
    public class TagGroup
    {
        public string[] tags;
        public int[] prefilledIndices;
        public int floorNumber;
    }
}

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Settings")]
    public int currentLevel = 1;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject teleporterPrefab;
    public GameObject tagPrefab;
    public GameObject tagSpotPrefab;

    [Header("Tag Settings")]
    public float tagSpacing = 2f;
    public float tagSpawnSafeZone = 3f;

    [Header("Teleporter Settings")]
    public float teleporterOffset = 2f;
    public float teleportHeight = 3f;

    private Vector3[] teleporterPositions;
    private GameObject player;
    private TagPlacementManager tagPlacementManager;

    // Niveles predefinidos
    private readonly LevelData[] predefinedLevels = new LevelData[]
    {
        // Nivel 1 - Introducción básica
    new LevelData {
        floorCount = 1,
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] { "<h1>", "Hello World", "</h1>" },
                prefilledIndices = new int[] { 1 }
            }
        }
    },
    // Nivel 2 - Introducción a anidamiento simple
    new LevelData {
        floorCount = 2,
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] { "<div>", "<p>", "Welcome", "</p>", "</div>" },
                prefilledIndices = new int[] { 2 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] { "<span>", "To HTML", "</span>" },
                prefilledIndices = new int[] { 1 }
            }
        }
    },
    // Nivel 3 - Multiple elementos por piso
    new LevelData {
        floorCount = 2,
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] { "<section>", "<h2>", "Title", "</h2>", "<p>", "Content", "</p>", "</section>" },
                prefilledIndices = new int[] { 2, 5 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] { "<article>", "<h3>", "Subtitle", "</h3>", "</article>" },
                prefilledIndices = new int[] { 2 }
            }
        }
    },
    // Nivel 4 - Tres pisos con elementos anidados
    new LevelData {
        floorCount = 3,
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] { "<main>", "<header>", "<h1>", "Main Title", "</h1>", "</header>", "</main>" },
                prefilledIndices = new int[] { 3 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] { "<nav>", "<ul>", "<li>", "Menu Item", "</li>", "</ul>", "</nav>" },
                prefilledIndices = new int[] { 3 }
            },
            new LevelData.TagGroup {
                floorNumber = 2,
                tags = new string[] { "<footer>", "<p>", "Copyright", "</p>", "</footer>" },
                prefilledIndices = new int[] { 2 }
            }
        }
    },
    // Nivel 5 - Cuatro pisos con estructura compleja
    new LevelData {
        floorCount = 4,
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] { "<html>", "<head>", "<title>", "Page Title", "</title>", "</head>", "</html>" },
                prefilledIndices = new int[] { 3 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] { "<body>", "<header>", "<nav>", "<ul>", "<li>", "Home", "</li>", "</ul>", "</nav>", "</header>", "</body>" },
                prefilledIndices = new int[] { 5 }
            },
            new LevelData.TagGroup {
                floorNumber = 2,
                tags = new string[] { "<main>", "<article>", "<h1>", "Article Title", "</h1>", "<p>", "Content", "</p>", "</article>", "</main>" },
                prefilledIndices = new int[] { 3, 6 }
            },
            new LevelData.TagGroup {
                floorNumber = 3,
                tags = new string[] { "<footer>", "<div>", "<small>", "Copyright 2025", "</small>", "</div>", "</footer>" },
                prefilledIndices = new int[] { 3 }
            }
        }
    }
};

    private void Start()
    {
        Debug.Log($"Iniciando generación del nivel {currentLevel}...");
        InitializeComponents();
        GenerateLevel();
    }

    private void InitializeComponents()
    {
        // Buscar o crear TagPlacementManager
        tagPlacementManager = FindObjectOfType<TagPlacementManager>();
        if (tagPlacementManager == null)
        {
            GameObject managerObject = new GameObject("TagPlacementManager");
            tagPlacementManager = managerObject.AddComponent<TagPlacementManager>();
            Debug.Log("TagPlacementManager creado automáticamente");
        }
    }

    private void GenerateLevel()
    {
        if (!ValidateLevel()) return;

        LevelData levelData = predefinedLevels[currentLevel - 1];
        teleporterPositions = new Vector3[levelData.floorCount * 2];
        Debug.Log($"Inicializando array de teleporters con tamaño {teleporterPositions.Length} para {levelData.floorCount} pisos");

        CleanupLevel();
        GenerateFloors(levelData);
        GenerateTagGroups(levelData);
        RepositionPlayer();

        int teleportersCreated = 0;
        foreach (var pos in teleporterPositions)
        {
            if (pos != Vector3.zero) teleportersCreated++;
        }
        Debug.Log($"Nivel {currentLevel} generado con {levelData.floorCount} pisos y {teleportersCreated} teleporters");
    }

    private bool ValidateLevel()
    {
        if (currentLevel < 1 || currentLevel > predefinedLevels.Length)
        {
            Debug.LogError($"Nivel {currentLevel} no existe!");
            return false;
        }
        return true;
    }

    private void CleanupLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void GenerateFloors(LevelData levelData)
    {
        Debug.Log($"Iniciando generación de {levelData.floorCount} pisos");

        for (int floor = 0; floor < levelData.floorCount; floor++)
        {
            Debug.Log($"Generando piso {floor}");
            GenerateFloor(floor, levelData);
            GenerateWalls(floor, levelData);
            GenerateTeleporters(floor, levelData);
        }
    }

    private void GenerateFloor(int floorNumber, LevelData levelData)
    {
        Vector3 position = new Vector3(0, floorNumber * levelData.floorHeight, 0);
        GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity, transform);
        floor.transform.localScale = new Vector3(levelData.floorDimensions.x, 0.5f, levelData.floorDimensions.y);
        floor.name = $"Floor_{floorNumber}";
    }

    private void GenerateWalls(int floorNumber, LevelData levelData)
    {
        float wallHeight = 3f;
        float halfWidth = levelData.floorDimensions.x / 2;
        float halfDepth = levelData.floorDimensions.y / 2;
        float y = floorNumber * levelData.floorHeight;

        // Crear las cuatro paredes
        CreateWall(new Vector3(0, y, halfDepth), new Vector3(levelData.floorDimensions.x, wallHeight, 1), Vector3.zero, floorNumber, "North");
        CreateWall(new Vector3(0, y, -halfDepth), new Vector3(levelData.floorDimensions.x, wallHeight, 1), Vector3.zero, floorNumber, "South");
        CreateWall(new Vector3(halfWidth, y, 0), new Vector3(1, wallHeight, levelData.floorDimensions.y), Vector3.zero, floorNumber, "East");
        CreateWall(new Vector3(-halfWidth, y, 0), new Vector3(1, wallHeight, levelData.floorDimensions.y), Vector3.zero, floorNumber, "West");
    }

    private void CreateWall(Vector3 position, Vector3 scale, Vector3 rotation, int floorNumber, string direction)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.Euler(rotation), transform);
        wall.transform.localScale = scale;
        wall.name = $"Wall_{floorNumber}_{direction}";
    }

    private void GenerateTeleporters(int floorNumber, LevelData levelData)
    {
        float wallOffset = 2f;
        float yPos = floorNumber * levelData.floorHeight + 0.34f;
        float zPos = -(levelData.floorDimensions.y / 2 - wallOffset);
        float spacing = 8f;
        float xPos1 = -spacing / 2;
        float xPos2 = spacing / 2;

        bool needsUpTeleporter = floorNumber < levelData.floorCount - 1; // Puede subir si no es el último piso
        bool needsDownTeleporter = floorNumber > 0; // Puede bajar si no es el primer piso

        Debug.Log($"Piso {floorNumber}: Necesita subida: {needsUpTeleporter}, Necesita bajada: {needsDownTeleporter}");

        // Si solo necesitamos un teleporter, lo centramos
        if (needsUpTeleporter && !needsDownTeleporter) // Solo subida
        {
            CreateTeleporter(
                new Vector3(0f, yPos, zPos),
                floorNumber,
                true,
                levelData,
                $"Teleporter_Up_{floorNumber}",
                floorNumber * 2
            );
        }
        else if (!needsUpTeleporter && needsDownTeleporter) // Solo bajada
        {
            CreateTeleporter(
                new Vector3(0f, yPos, zPos),
                floorNumber,
                false,
                levelData,
                $"Teleporter_Down_{floorNumber}",
                floorNumber * 2 + 1
            );
        }
        else if (needsUpTeleporter && needsDownTeleporter) // Ambos
        {
            // Teleporter de subida (izquierda)
            CreateTeleporter(
                new Vector3(xPos1, yPos, zPos),
                floorNumber,
                true,
                levelData,
                $"Teleporter_Up_{floorNumber}",
                floorNumber * 2
            );

            // Teleporter de bajada (derecha)
            CreateTeleporter(
                new Vector3(xPos2, yPos, zPos),
                floorNumber,
                false,
                levelData,
                $"Teleporter_Down_{floorNumber}",
                floorNumber * 2 + 1
            );
        }
    }

    private void CreateTeleporter(Vector3 position, int currentFloor, bool isUpward, LevelData levelData, string name, int positionIndex)
    {
        if (teleporterPrefab == null)
        {
            Debug.LogError("¡Teleporter prefab no asignado!");
            return;
        }

        GameObject teleporter = Instantiate(teleporterPrefab, position, Quaternion.identity, transform);
        if (teleporter == null)
        {
            Debug.LogError("Error al instanciar el teleporter");
            return;
        }

        teleporter.name = name;

        // Configurar colores del teleporter
        Renderer renderer = teleporter.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Crear un nuevo material para no afectar al material del prefab
            Material teleporterMaterial = new Material(renderer.material);

            // Asignar colores según el tipo de teleporter
            if (isUpward)
            {
                // Color azul brillante para subida
                teleporterMaterial.color = new Color(0.3f, 0.7f, 1f, 0.8f);
            }
            else
            {
                // Color verde brillante para bajada
                teleporterMaterial.color = new Color(0.3f, 1f, 0.5f, 0.8f);
            }

            // Asegurarnos de que el material sea algo transparente
            teleporterMaterial.SetFloat("_Mode", 3); // Transparent mode
            teleporterMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            teleporterMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            teleporterMaterial.SetInt("_ZWrite", 0);
            teleporterMaterial.DisableKeyword("_ALPHATEST_ON");
            teleporterMaterial.EnableKeyword("_ALPHABLEND_ON");
            teleporterMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            teleporterMaterial.renderQueue = 3000;

            renderer.material = teleporterMaterial;
        }

        var teleporterScript = teleporter.GetComponent<Teleporter>();
        if (teleporterScript == null)
        {
            teleporterScript = teleporter.AddComponent<Teleporter>();
        }

        if (teleporterScript != null)
        {
            teleporterScript.targetHeight = isUpward ?
                (currentFloor + 1) * levelData.floorHeight :
                (currentFloor - 1) * levelData.floorHeight;

            teleporterScript.teleportHeight = 1f;
        }

        teleporterPositions[positionIndex] = teleporter.transform.position;

        var collider = teleporter.GetComponent<SphereCollider>();
        if (collider == null)
        {
            collider = teleporter.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 1.5f;
        }

        Debug.Log($"Creado teleporter {name} - Tipo: {(isUpward ? "Subida" : "Bajada")} - Color: {(isUpward ? "Azul" : "Verde")}");
    }

    private void GenerateTagGroups(LevelData levelData)
    {
        foreach (var tagGroup in levelData.tagGroups)
        {
            GenerateTagGroupElements(tagGroup, levelData);
        }
    }

    private void GenerateTagGroupElements(LevelData.TagGroup tagGroup, LevelData levelData)
    {
        float floorY = tagGroup.floorNumber * levelData.floorHeight;
        float wallOffset = 2f; // Distancia desde el muro
        float spotHeight = 0.3f; // Altura del spot
        float spotSpacing = 4f; // Separación entre spots

        // Colocar los spots cerca del muro norte
        float wallZ = levelData.floorDimensions.y / 2 - wallOffset;
        float totalWidth = (tagGroup.tags.Length - 1) * spotSpacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < tagGroup.tags.Length; i++)
        {
            // Crear spot
            Vector3 spotPosition = new Vector3(
                startX + (i * spotSpacing),
                floorY + spotHeight,
                wallZ
            );

            CreateSpot(spotPosition, tagGroup, i, levelData);
        }
    }

    private void CreateSpot(Vector3 spotPosition, LevelData.TagGroup tagGroup, int index, LevelData levelData)
    {
        GameObject spot = Instantiate(tagSpotPrefab, spotPosition, Quaternion.identity, transform);
        spot.name = $"TagSpot_{tagGroup.floorNumber}_{index}";

        // Asegurarnos de que tiene el componente TagPlacementSpot
        var tagSpotComponent = spot.GetComponent<TagPlacementSpot>();
        if (tagSpotComponent == null)
        {
            tagSpotComponent = spot.AddComponent<TagPlacementSpot>();
            Debug.Log($"TagPlacementSpot añadido a {spot.name}");
        }

        bool isPrefilled = Array.Exists(tagGroup.prefilledIndices, i => i == index);
        ConfigureTagSpot(tagSpotComponent, tagGroup.tags[index], isPrefilled);

        // Crear tag
        Vector3 tagPosition = isPrefilled ?
            spotPosition + Vector3.up * 0.2f :
            GetRandomPositionOnFloor(tagGroup.floorNumber, levelData);

        CreateTag(tagGroup.tags[index], tagPosition, isPrefilled, tagGroup.floorNumber, index);

        // Registrar en el manager
        RegisterSpotWithManager(spot.transform, tagGroup.tags[index], isPrefilled);
    }

    private void ConfigureTagSpot(TagPlacementSpot spotComponent, string expectedTag, bool isPrefilled)
    {
        spotComponent.expectedTag = expectedTag;
        spotComponent.isPreFilled = isPrefilled;
        Debug.Log($"Configurado spot para tag: {expectedTag}, Prefilled: {isPrefilled}");
    }

    private void CreateTag(string tagText, Vector3 position, bool isPrefilled, int floorNumber, int index)
    {
        if (tagPrefab == null)
        {
            Debug.LogError("tagPrefab no está asignado en el LevelGenerator");
            return;
        }

        GameObject tag = Instantiate(tagPrefab, position, Quaternion.identity, transform);
        tag.name = isPrefilled ? $"PrefillTag_{floorNumber}_{index}" : $"Tag_{floorNumber}_{index}";

        var htmlTag = tag.GetComponent<HTMLTagObject>();
        if (htmlTag == null)
        {
            htmlTag = tag.AddComponent<HTMLTagObject>();
            Debug.Log($"HTMLTagObject añadido a {tag.name}");
        }

        htmlTag.SetTagText(tagText);
        if (isPrefilled)
        {
            htmlTag.SetAsPrefilled(true);
        }

        Debug.Log($"Creado tag {(isPrefilled ? "prefijado" : "movible")}: {tagText} en posición {position}");

        // Verificar que el tag se creó correctamente
        if (tag == null || !tag.activeInHierarchy)
        {
            Debug.LogError($"Error al crear el tag {tagText}");
        }
    }

    private void RegisterSpotWithManager(Transform spotTransform, string expectedTag, bool isPrefilled)
    {
        if (tagPlacementManager != null)
        {
            var placementSpot = new TagPlacementManager.TagPlacementSpot
            {
                spotTransform = spotTransform,
                expectedTag = expectedTag,
                isPreFilled = isPrefilled,
                isOccupied = isPrefilled
            };
            tagPlacementManager.placementSpots.Add(placementSpot);
        }
    }

    private Vector3 GetRandomPositionOnFloor(int floorNumber, LevelData levelData)
    {
        float xRange = levelData.floorDimensions.x / 2 - tagSpawnSafeZone;
        float zMin = -(levelData.floorDimensions.y / 4); // Limitamos el spawn a la mitad frontal del piso
        float zMax = (levelData.floorDimensions.y / 2) - tagSpawnSafeZone;
        float height = floorNumber * levelData.floorHeight + 0.5f;

        // Evitar posiciones cercanas a los teleporters
        Vector3 randomPos;
        do
        {
            randomPos = new Vector3(
                UnityEngine.Random.Range(-xRange, xRange),
                height,
                UnityEngine.Random.Range(zMin, zMax)
            );
        } while (IsTooCloseToTeleporters(randomPos));

        return randomPos;
    }

    private bool IsTooCloseToTeleporters(Vector3 position)
    {
        const float MIN_DISTANCE_TO_TELEPORTER = 4f; // Aumentamos un poco la distancia mínima

        if (teleporterPositions == null) return false;

        foreach (var teleporterPos in teleporterPositions)
        {
            if (teleporterPos != Vector3.zero &&
                Vector3.Distance(new Vector3(position.x, teleporterPos.y, position.z), teleporterPos) < MIN_DISTANCE_TO_TELEPORTER)
            {
                return true;
            }
        }
        return false;
    }

    private void RepositionPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No se encontró el jugador en la escena!");
            return;
        }

        Vector3 spawnPosition = new Vector3(0, 1f, 0);

        var charController = player.GetComponent<CharacterController>();
        if (charController != null)
        {
            charController.enabled = false;
            player.transform.position = spawnPosition;
            charController.enabled = true;
        }
        else
        {
            player.transform.position = spawnPosition;
        }

        Debug.Log($"Jugador reposicionado en {spawnPosition}");
    }
}