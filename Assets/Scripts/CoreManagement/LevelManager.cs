using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private LevelData[] levels;
    [SerializeField] private float floorHeight = 10f;

    [Header("Prefabs")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject teleporterPrefab;
    [SerializeField] private GameObject tagPrefab;
    [SerializeField] private GameObject tagSpotPrefab;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject upwardIndicatorPrefab;
    [SerializeField] private GameObject downwardIndicatorPrefab;

    [Header("Generation Settings")]
    [SerializeField] private float tagSpacing = 4f;
    [SerializeField] private float tagSpawnSafeZone = 3f;
    [SerializeField] private float wallHeight = 5f;
    [SerializeField] private Vector2 defaultWallSize = new Vector2(2, 2);

    [Header("Teleporter Settings")]
    [SerializeField] private float teleporterOffset = 2f;
    [SerializeField] private float teleporterSpacing = 8f;
    [SerializeField] private float teleportHeight = 1f;

    private readonly LevelData[] predefinedLevels = new LevelData[]
{
    // Nivel 1 - Introducción básica HTML
    new LevelData {
        floorCount = 1,
        floorDimensions = new Vector2(40f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] { "<h1>", "Mi Primera Página", "</h1>" },
                prefilledIndices = new int[] { 1 }
            }
        }
    },

    // Nivel 2 - Párrafos simples
    new LevelData {
        floorCount = 2,
        floorDimensions = new Vector2(40f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] { "<div>", "<h2>", "Sobre HTML", "</h2>", "</div>" },
                prefilledIndices = new int[] { 2 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] { "<p>", "Aprendiendo etiquetas HTML", "</p>" },
                prefilledIndices = new int[] { 1 }
            }
        }
    },

    // Nivel 3 - Artículo Simple
    new LevelData {
        floorCount = 2,
        floorDimensions = new Vector2(50f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] {
                    "<article>", "<h1>", "Mi Blog Personal", "</h1>",
                    "<h2>", "Primera Entrada", "</h2>", "</article>"
                },
                prefilledIndices = new int[] { 2, 5 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] {
                    "<p>", "Bienvenidos a mi blog", "</p>",
                    "<p>", "¡Espero que os guste!", "</p>"
                },
                prefilledIndices = new int[] { 1, 4 }
            }
        }
    },

    // Nivel 4 - Navegación Básica
    new LevelData {
        floorCount = 3,
        floorDimensions = new Vector2(50f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] {
                    "<header>", "<h1>", "Mi Sitio Web", "</h1>", "</header>"
                },
                prefilledIndices = new int[] { 2 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] {
                    "<nav>", "<ul>", "<li>", "Inicio", "</li>",
                    "<li>", "Contacto", "</li>", "</ul>", "</nav>"
                },
                prefilledIndices = new int[] { 3, 6 }
            },
            new LevelData.TagGroup {
                floorNumber = 2,
                tags = new string[] {
                    "<footer>", "<p>", "© 2025 - Todos los derechos reservados", "</p>", "</footer>"
                },
                prefilledIndices = new int[] { 2 }
            }
        }
    },

    // Nivel 5 - Blog Post Completo
    new LevelData {
        floorCount = 3,
        floorDimensions = new Vector2(50f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] {
                    "<article>", "<header>", "<h1>", "Las Maravillas de HTML", "</h1>", "</header>", "</article>"
                },
                prefilledIndices = new int[] { 3 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] {
                    "<section>", "<h2>", "Introducción", "</h2>",
                    "<p>", "HTML es fascinante", "</p>", "</section>"
                },
                prefilledIndices = new int[] { 2, 5 }
            },
            new LevelData.TagGroup {
                floorNumber = 2,
                tags = new string[] {
                    "<section>", "<h2>", "Conclusión", "</h2>",
                    "<p>", "¡Sigamos aprendiendo!", "</p>", "</section>"
                },
                prefilledIndices = new int[] { 2, 5 }
            }
        }
    },

    // Nivel 6 - Formulario
    new LevelData {
        floorCount = 3,
        floorDimensions = new Vector2(50f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] {
                    "<form>", "<h2>", "Formulario de Contacto", "</h2>", "</form>"
                },
                prefilledIndices = new int[] { 2 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] {
                    "<label>", "Nombre:", "</label>",
                    "<input>", "<br>",
                    "<label>", "Email:", "</label>"
                },
                prefilledIndices = new int[] { 1, 3, 6 }
            },
            new LevelData.TagGroup {
                floorNumber = 2,
                tags = new string[] {
                    "<textarea>", "Tu mensaje aquí", "</textarea>",
                    "<button>", "Enviar", "</button>"
                },
                prefilledIndices = new int[] { 1, 4 }
            }
        }
    },

    // Nivel 7 - Menú Multinivel
    new LevelData {
        floorCount = 3,
        floorDimensions = new Vector2(50f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] {
                    "<nav>", "<ul>", "<li>", "Productos", "</li>", "</ul>", "</nav>"
                },
                prefilledIndices = new int[] { 3 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] {
                    "<ul>", "<li>", "Categoría 1", "</li>",
                    "<ul>", "<li>", "Subcategoría", "</li>", "</ul>", "</ul>"
                },
                prefilledIndices = new int[] { 2, 6 }
            },
            new LevelData.TagGroup {
                floorNumber = 2,
                tags = new string[] {
                    "<ul>", "<li>", "Categoría 2", "</li>",
                    "<ul>", "<li>", "Subcategoría", "</li>", "</ul>", "</ul>"
                },
                prefilledIndices = new int[] { 2, 6 }
            }
        }
    },

    // Nivel 8 - Tabla de Datos
    new LevelData {
        floorCount = 3,
        floorDimensions = new Vector2(60f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] {
                    "<table>", "<thead>", "<tr>", "<th>", "Nombre", "</th>",
                    "<th>", "Edad", "</th>", "</tr>", "</thead>", "</table>"
                },
                prefilledIndices = new int[] { 4, 7 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] {
                    "<tbody>", "<tr>", "<td>", "Ana", "</td>",
                    "<td>", "25", "</td>", "</tr>", "</tbody>"
                },
                prefilledIndices = new int[] { 3, 6 }
            },
            new LevelData.TagGroup {
                floorNumber = 2,
                tags = new string[] {
                    "<tfoot>", "<tr>", "<td>", "Total", "</td>",
                    "<td>", "1 persona", "</td>", "</tr>", "</tfoot>"
                },
                prefilledIndices = new int[] { 3, 6 }
            }
        }
    },

    // Nivel 9 - Página Web Completa
    new LevelData {
        floorCount = 4,
        floorDimensions = new Vector2(60f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] {
                    "<html>", "<head>", "<title>", "Mi Portafolio", "</title>",
                    "</head>", "</html>"
                },
                prefilledIndices = new int[] { 3 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] {
                    "<body>", "<header>", "<nav>", "<a>", "Inicio", "</a>",
                    "<a>", "Proyectos", "</a>", "</nav>", "</header>", "</body>"
                },
                prefilledIndices = new int[] { 4, 7 }
            },
            new LevelData.TagGroup {
                floorNumber = 2,
                tags = new string[] {
                    "<main>", "<section>", "<h1>", "Mis Trabajos", "</h1>",
                    "<article>", "Proyecto 1", "</article>", "</section>", "</main>"
                },
                prefilledIndices = new int[] { 3, 6 }
            },
            new LevelData.TagGroup {
                floorNumber = 3,
                tags = new string[] {
                    "<footer>", "<div>", "<p>", "Contacto: email@ejemplo.com", "</p>",
                    "</div>", "</footer>"
                },
                prefilledIndices = new int[] { 3 }
            }
        }
    },

    // Nivel 10 - Página Web Avanzada
    new LevelData {
        floorCount = 4,
        floorDimensions = new Vector2(60f, 40f),
        tagGroups = new LevelData.TagGroup[] {
            new LevelData.TagGroup {
                floorNumber = 0,
                tags = new string[] {
                    "<html>", "<head>", "<meta>", "<title>", "Tienda en Línea", "</title>",
                    "<link>", "</head>", "</html>"
                },
                prefilledIndices = new int[] { 4 }
            },
            new LevelData.TagGroup {
                floorNumber = 1,
                tags = new string[] {
                    "<body>", "<header>", "<nav>", "<div>", "Logo", "</div>",
                    "<ul>", "<li>", "Carrito", "</li>", "</ul>", "</nav>", "</header>", "</body>"
                },
                prefilledIndices = new int[] { 4, 8 }
            },
            new LevelData.TagGroup {
                floorNumber = 2,
                tags = new string[] {
                    "<main>", "<aside>", "<h2>", "Filtros", "</h2>", "</aside>",
                    "<section>", "<h1>", "Productos Destacados", "</h1>", "</section>", "</main>"
                },
                prefilledIndices = new int[] { 3, 8 }
            },
            new LevelData.TagGroup {
                floorNumber = 3,
                tags = new string[] {
                    "<footer>", "<section>", "<h3>", "Newsletter", "</h3>",
                    "<form>", "<input>", "<button>", "Suscribirse", "</button>",
                    "</form>", "</section>", "</footer>"
                },
                prefilledIndices = new int[] { 3, 8 }
            }
        }
    }
};

    private Vector3[] teleporterPositions;
    public int CurrentLevel { get; private set; } = 1;

    private void Start()
    {
        levels = predefinedLevels;
    }

    public void GenerateLevel(int levelIndex)
    {
        if (!ValidateLevel(levelIndex)) return;

        CleanupCurrentLevel();
        CurrentLevel = levelIndex;
        LevelData currentLevel = levels[levelIndex - 1];
        teleporterPositions = new Vector3[currentLevel.floorCount * 2];

        GenerateFloors(currentLevel);
        GenerateTagSpots(currentLevel);
        RepositionPlayer();

        if (GameManager.Instance.TimerManager != null)
        {
            GameManager.Instance.TimerManager.gameObject.SetActive(true);
        }

        GameManager.Instance.UIManager.ShowTimer();

        EventSystem.RaiseLevelStarted(levelIndex);
    }

    private bool ValidateLevel(int levelIndex) =>
        levelIndex >= 1 && levelIndex <= levels.Length;

    private void CleanupCurrentLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GameManager.Instance.TagManager.ClearSpots();
    }

    private void GenerateFloors(LevelData levelData)
    {
        for (int floor = 0; floor < levelData.floorCount; floor++)
        {
            CreateFloor(floor);
            CreateWalls(floor);
            CreateTeleporters(floor, levelData.floorCount);
        }
    }

    private void CreateFloor(int floorNumber)
    {
        Vector3 position = new Vector3(0, floorNumber * floorHeight, 0);
        GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity, transform);
        Vector2 dimensions = levels[CurrentLevel - 1].floorDimensions;
        floor.transform.localScale = new Vector3(dimensions.x, 0.5f, dimensions.y);
        floor.name = $"Floor_{floorNumber}";
    }

    private void CreateWalls(int floorNumber)
    {
        Vector2 dimensions = levels[CurrentLevel - 1].floorDimensions;
        float halfWidth = dimensions.x / 2;
        float halfDepth = dimensions.y / 2;
        float y = floorNumber * floorHeight;

        CreateWall(new Vector3(0, y, halfDepth), new Vector3(dimensions.x, wallHeight, 1), Vector3.zero, floorNumber, "North");
        CreateWall(new Vector3(0, y, -halfDepth), new Vector3(dimensions.x, wallHeight, 1), Vector3.zero, floorNumber, "South");
        CreateWall(new Vector3(halfWidth, y, 0), new Vector3(1, wallHeight, dimensions.y), Vector3.zero, floorNumber, "East");
        CreateWall(new Vector3(-halfWidth, y, 0), new Vector3(1, wallHeight, dimensions.y), Vector3.zero, floorNumber, "West");
    }

    private void CreateWall(Vector3 position, Vector3 scale, Vector3 rotation, int floorNumber, string direction)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.Euler(rotation), transform);
        wall.transform.localScale = scale;
        wall.name = $"Wall_{floorNumber}_{direction}";
        ConfigureWallMaterial(wall, scale);
    }

    private void ConfigureWallMaterial(GameObject wall, Vector3 scale)
    {
        if (wall.TryGetComponent<Renderer>(out var renderer) && renderer.sharedMaterial != null)
        {
            Material wallMaterial = new Material(renderer.sharedMaterial);
            float textureRepeatX = Mathf.Max(scale.x, scale.z) / defaultWallSize.x;
            float textureRepeatY = scale.y / defaultWallSize.y;
            wallMaterial.mainTextureScale = new Vector2(textureRepeatX, textureRepeatY);
            renderer.material = wallMaterial;
        }
    }

    private void CreateTeleporters(int floorNumber, int totalFloors)
    {
        Vector2 dimensions = levels[CurrentLevel - 1].floorDimensions;
        float yPos = floorNumber * floorHeight + 0.34f;
        float zPos = -(dimensions.y / 2 - teleporterOffset);
        bool needsUpTeleporter = floorNumber < totalFloors - 1;
        bool needsDownTeleporter = floorNumber > 0;

        if (needsUpTeleporter && !needsDownTeleporter)
        {
            SpawnTeleporter(new Vector3(0f, yPos, zPos), floorNumber, true, $"Teleporter_Up_{floorNumber}", floorNumber * 2);
        }
        else if (!needsUpTeleporter && needsDownTeleporter)
        {
            SpawnTeleporter(new Vector3(0f, yPos, zPos), floorNumber, false, $"Teleporter_Down_{floorNumber}", floorNumber * 2 + 1);
        }
        else if (needsUpTeleporter && needsDownTeleporter)
        {
            SpawnTeleporter(new Vector3(-teleporterSpacing / 2, yPos, zPos), floorNumber, true, $"Teleporter_Up_{floorNumber}", floorNumber * 2);
            SpawnTeleporter(new Vector3(teleporterSpacing / 2, yPos, zPos), floorNumber, false, $"Teleporter_Down_{floorNumber}", floorNumber * 2 + 1);
        }
    }

    private void SpawnTeleporter(Vector3 position, int currentFloor, bool isUpward, string name, int positionIndex)
    {
        GameObject teleporter = Instantiate(teleporterPrefab, position, Quaternion.identity, transform);
        teleporter.name = name;
        ConfigureTeleporter(teleporter, currentFloor, isUpward);
        teleporterPositions[positionIndex] = teleporter.transform.position;

        float indicatorHeight = isUpward ? 0.5f : 3.0f;
        Vector3 indicatorPosition = position + Vector3.up * indicatorHeight;

        GameObject indicator = Instantiate(
            isUpward ? upwardIndicatorPrefab : downwardIndicatorPrefab,
            indicatorPosition,
            Quaternion.identity,
            teleporter.transform
        );
        indicator.AddComponent<TeleporterIndicator>();
    }

    private void ConfigureTeleporter(GameObject teleporter, int currentFloor, bool isUpward)
    {
        var teleporterComponent = teleporter.GetComponent<Teleporter>() ?? teleporter.AddComponent<Teleporter>();
        teleporterComponent.targetHeight = (currentFloor + (isUpward ? 1 : -1)) * floorHeight;
        teleporterComponent.teleportHeight = teleportHeight;

        Material teleporterMaterial = new Material(teleporter.GetComponent<Renderer>().sharedMaterial);
        teleporterMaterial.color = isUpward ?
            new Color(0.3f, 0.7f, 1f, 0.8f) :
            new Color(0.3f, 1f, 0.5f, 0.8f);
        ConfigureTransparentMaterial(teleporterMaterial);
        teleporter.GetComponent<Renderer>().material = teleporterMaterial;
    }

    private void GenerateTagSpots(LevelData levelData)
    {
        foreach (var tagGroup in levelData.tagGroups)
        {
            float floorY = tagGroup.floorNumber * floorHeight;
            float wallZ = levelData.floorDimensions.y / 2 - 2f;
            float totalWidth = (tagGroup.tags.Length - 1) * tagSpacing;
            float startX = -totalWidth / 2;

            for (int i = 0; i < tagGroup.tags.Length; i++)
            {
                Vector3 spotPosition = new Vector3(
                    startX + (i * tagSpacing),
                    floorY + 0.3f,
                    wallZ
                );

                CreateTagSpot(spotPosition, tagGroup, i, levelData);
            }
        }
    }

    private void CreateTagSpot(Vector3 position, LevelData.TagGroup tagGroup, int index, LevelData levelData)
    {
        GameObject spot = Instantiate(tagSpotPrefab, position, Quaternion.identity, transform);
        spot.name = $"TagSpot_{tagGroup.floorNumber}_{index}";

        var tagSpot = spot.GetComponent<TagSpot>() ?? spot.AddComponent<TagSpot>();
        bool isPrefilled = Array.Exists(tagGroup.prefilledIndices, i => i == index);

        tagSpot.Initialize(tagGroup.tags[index], isPrefilled);
        GameManager.Instance.TagManager.RegisterSpot(tagSpot);

        if (!isPrefilled)
        {
            CreateTag(tagGroup.tags[index], GetRandomSpawnPosition(tagGroup.floorNumber), false);
        }
        else
        {
            CreateTag(tagGroup.tags[index], position + Vector3.up * 0.31f, true);
        }
    }

    private Vector3 GetRandomSpawnPosition(int floorNumber)
    {
        Vector2 dimensions = levels[CurrentLevel - 1].floorDimensions;
        float xRange = dimensions.x / 2 - tagSpawnSafeZone;
        float zMin = -(dimensions.y / 4);
        float zMax = (dimensions.y / 2) - tagSpawnSafeZone;
        float height = floorNumber * floorHeight + 0.5f;

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

    private void CreateTag(string tagText, Vector3 position, bool isPrefilled)
    {
        GameObject tag = Instantiate(tagPrefab, position, Quaternion.identity, transform);
        var tagObject = tag.GetComponent<TagObject>() ?? tag.AddComponent<TagObject>();
        tagObject.Initialize(tagText, isPrefilled);
    }

    private bool IsTooCloseToTeleporters(Vector3 position, float minDistance = 4f)
    {
        if (teleporterPositions == null) return false;

        foreach (var teleporterPos in teleporterPositions)
        {
            if (teleporterPos != Vector3.zero &&
                Vector3.Distance(new Vector3(position.x, teleporterPos.y, position.z), teleporterPos) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    private void RepositionPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 spawnPosition = new Vector3(0, 1f, 0);
        if (player.TryGetComponent<CharacterController>(out var charController))
        {
            charController.enabled = false;
            player.transform.position = spawnPosition;
            charController.enabled = true;
        }
        else
        {
            player.transform.position = spawnPosition;
        }
    }

    private void ConfigureTransparentMaterial(Material material)
    {
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}