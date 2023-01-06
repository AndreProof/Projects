let scene,
    camera,
    renderer,

    isMouseDown = false,
    onMouseDownPosition,
    radious = 1600,
    theta = 45,
    onMouseDownTheta = 45,
    phi = 60,
    onMouseDownPhi = 60,
    selectedMesh,

    gui,

    isPlacing = false,

    figureVisualParameters = {
        fill: true,
        frame: false
    },

    visualParameters = {
        figure: true,
        shell: false,
        equidistant: true,
        closed: false
    },

    objectParameters = {
        angleX: 0,
        angleY: 0,
        angleZ: 0,
        color: 0xFFFFFFFF
    },

    placingParameters = {
        maxWidth: 250,
        maxHeight: 250,
        rotationAngle: 1,
        techDist: 5
    },

    objectParametersFolder,
    objectsFolder,
    placingFolder;

function initGUI() {

    if (gui != undefined) {
        gui.destroy();
    }

    gui = new dat.gui.GUI();

    let figureVisualParametersFolder;

    if (isPlacing) {
        let visualParametersFolder = gui.addFolder("Тип отображения");
        visualParametersFolder.add(visualParameters, 'figure').name('Фигура').listen().onChange(() => {
            changeFigureVisualization();
            if (visualParameters.figure === true) {
                figureVisualParametersFolder.show();
                figureVisualParameters.closed = false;
            } else {
                figureVisualParametersFolder.hide();
            }
            drawAll();
        });

        visualParametersFolder.add(visualParameters, 'equidistant').name('Эквидистанта').listen().onChange(() => {
            drawAll();
        });

        visualParametersFolder.closed = false;
    }

    figureVisualParametersFolder = gui.addFolder("Визуализация");
    figureVisualParametersFolder.add(figureVisualParameters, 'fill').name('Заливка').listen().onChange(() => {
        figureVisualParameters.frame = !figureVisualParameters.fill;
        changeFigureVisualization();
    });
    figureVisualParametersFolder.add(figureVisualParameters, 'frame').name('Каркас').listen().onChange(() => {
        figureVisualParameters.fill = !figureVisualParameters.frame;
        changeFigureVisualization();
    });
    figureVisualParametersFolder.closed = false;

    let addBtn = { add: loadSTL };
    gui.add(addBtn, 'add').name('Загрузить объект');

    if (!isPlacing) {
        objectsFolder = gui.addFolder("Объекты");

        let clearBtn = { clear: function () { clear(objectsFolder) } };
        objectsFolder.add(clearBtn, 'clear').name('Очистить');
        objectsFolder.closed = false;
        initGUIFigures();
    }

    placingFolder = gui.addFolder("Размещение");
    placingFolder.add(placingParameters, 'rotationAngle', 0, 360, 1).name('Угол поворота').listen().onChange(() => {
        postPlaceParameters();
    });
    placingFolder.add(placingParameters, 'maxWidth', 0, 1000, 1).name('Макс. ширина').listen().onChange(() => {
        postPlaceParameters();
    });
    placingFolder.add(placingParameters, 'maxHeight', 0, 1000, 1).name('Макс. высота').listen().onChange(() => {
        postPlaceParameters();
    });
    placingFolder.add(placingParameters, 'techDist', 0, 50, 1).name('Тех. ограничение').listen().onChange(() => {
        postPlaceParameters();
    });
    let placeBtn = { place: place };
    placingFolder.add(placeBtn, 'place').name('Разместить');
    placingFolder.closed = false;

    postPlaceParameters();
}

function initGL() {
    scene = new THREE.Scene();
    camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 15000);

    setCamera();

    renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setClearColor('0x333333');
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(window.innerWidth, window.innerHeight);
    document.getElementById("webgl").appendChild(renderer.domElement);

    onMouseDownPosition = new THREE.Vector2();

    document.addEventListener('mousemove', onDocumentMouseMove, false);
    document.addEventListener('mousedown', onDocumentMouseDown, false);
    document.addEventListener('mouseup', onDocumentMouseUp, false);
    document.addEventListener('mousewheel', onDocumentMouseWheel, false);

    const light = new THREE.DirectionalLight(0xdfebff, 1);
    light.position.set(50, 200, 100);
    light.position.multiplyScalar(1.3);

    light.castShadow = true;

    light.shadow.mapSize.width = 1024;
    light.shadow.mapSize.height = 1024;

    const d = 300;

    light.shadow.camera.left = - d;
    light.shadow.camera.right = d;
    light.shadow.camera.top = d;
    light.shadow.camera.bottom = - d;

    light.shadow.camera.far = 1000;
    scene.add(light);

    let max = 1000;

    let xMaterial = new THREE.LineBasicMaterial({ color: 'brown' });
    let yMaterial = new THREE.LineBasicMaterial({ color: 'green' });
    let zMaterial = new THREE.LineBasicMaterial({ color: 'blue' });

    drawLine(scene, xMaterial, new THREE.Vector3(0, 0, 0), new THREE.Vector3(max, 0, 0));
    drawLine(scene, yMaterial, new THREE.Vector3(0, 0, 0), new THREE.Vector3(0, max, 0));
    drawLine(scene, zMaterial, new THREE.Vector3(0, 0, 0), new THREE.Vector3(0, 0, max));

    drawArea();

    render();
}

function initGUIFigures() {

    let xhr = new XMLHttpRequest();
    xhr.open('GET', 'Home/GetFigures', true);
    xhr.onload = function () {
        const figures = JSON.parse(xhr.response);

        for (let figId in figures) {
            let figure = figures[figId];

            let isSelected = figure.IsSelected;
            let id = parseInt(figure.Id);
            let name = figure.Name;
            let btnName = id + ": " + name;

            if (isSelected) {
                let figureFolder = objectsFolder.addFolder(btnName);
                figureFolder.add(objectParameters, 'angleX', 0, 360, 1).name('X').listen().onChange(() => {
                    rotateSelectedMesh();
                    postRotate();
                });
                figureFolder.add(objectParameters, 'angleY', 0, 360, 1).name('Y').listen().onChange(() => {
                    rotateSelectedMesh();
                    postRotate();
                });
                figureFolder.add(objectParameters, 'angleZ', 0, 360, 1).name('Z').listen().onChange(() => {
                    rotateSelectedMesh();
                    postRotate();
                });
                figureFolder.addColor(objectParameters, 'color').name('Цвет').listen().onChange(() => {
                    colorSelectedMesh();
                    postColor();
                });
                figureFolder.closed = false;

            } else {
                let selectBtn = {
                    add: function() {
                        setSelectedItem(id);
                        initGUI();
                    }
                };

                objectsFolder.add(selectBtn, 'add').name(btnName);
            }
        }
    };
    xhr.send();
}

// Управляющие кнопки.
function clear(folder) {
    let xhr = new XMLHttpRequest();
    xhr.open('POST', 'Home/ClearScene', false);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.addEventListener("load", drawAll, false);
    xhr.send();

    while (folder.__controllers.length > 1) {
        let item = folder.__controllers[folder.__controllers.length-1];
        folder.remove(item);
    }

    objectsFolder.hide();
    placingFolder.hide();
}

function place() {
    let xhr = new XMLHttpRequest();
    xhr.open('POST', 'Home/PlaceScene', true);
    xhr.onloadstart = function() {
        progress(true);
    };
    xhr.onload = function () {
        const answer = xhr.response;

        progress(false);

        alert(answer);

        if (answer == 'Размещение не удалось') {
            return;
        }

        isPlacing = true;

        initGUI();

        drawAll();
    };
    xhr.send();
}

// Загрузка файла.
function loadSTL() {
    if (window.File && window.FileReader && window.FileList && window.Blob) {
        let input = document.createElement('input');
        input.type = 'file';
        input.accept = ".stl";
        input.onchange = e => {
            let file = e.target.files[0];

            let reader = new FileReader();
            reader.readAsText(file, 'UTF-8');
            let jsonContent;
            reader.onload = readerEvent => {

                progress(true);

                try {
                    let content = readerEvent.target.result.split('\n');
                    let filteredContent = content.filter(function (str) {
                        return str != "";
                    }).map(s => s.trim());;
                    jsonContent = parseFile(filteredContent);
                }
                catch (e)
                {
                    progress(false);

                    alert('Невозможно загрузить файл (неправильная структура файла)');
                    return;
                }

                let xhr = new XMLHttpRequest();
                xhr.open('POST', 'Home/PostFigure', true);
                xhr.onload = function (e) {
                    objectsFolder.show();
                    placingFolder.show();
                    initGUI();

                    isPlacing = false;

                    drawAll();

                    progress(false);
                };
                xhr.send(jsonContent);
            }
        }
        input.click();
    } else {
        progress(false);
        alert('Открытие файлов в вашем браузере невозможно.');
    }
}

function parseFile(content){
    try {

        let nWords = ["", "facet", "normal", "vertex"];

        var myObject = {};
        myObject.Meshes = new Array();

        for (let i = 0; i < content.length; i++) {
            let words = content[i].split(' ');

            if (words[0] === "solid")
            {
                myObject.Name = words[1];
            }

            if (words[0] === "facet")
            {
                let clearWords = words.filter(e => !nWords.includes(e));

                let fstVertexWords = content[i + 2].split(' ').filter(e => !nWords.includes(e));
                let sndVertexWords = content[i + 3].split(' ').filter(e => !nWords.includes(e));
                let thrVertexWords = content[i + 4].split(' ').filter(e => !nWords.includes(e));

                let meshObject = {};
                meshObject.FstPoint = {};
                meshObject.FstPoint.X = parseFloat(fstVertexWords[0]);
                meshObject.FstPoint.Y = parseFloat(fstVertexWords[1]);
                meshObject.FstPoint.Z = parseFloat(fstVertexWords[2]);

                meshObject.SndPoint = {};
                meshObject.SndPoint.X = parseFloat(sndVertexWords[0]);
                meshObject.SndPoint.Y = parseFloat(sndVertexWords[1]);
                meshObject.SndPoint.Z = parseFloat(sndVertexWords[2]);

                meshObject.TrdPoint = {};
                meshObject.TrdPoint.X = parseFloat(thrVertexWords[0]);
                meshObject.TrdPoint.Y = parseFloat(thrVertexWords[1]);
                meshObject.TrdPoint.Z = parseFloat(thrVertexWords[2]);

                meshObject.Normal = {};
                meshObject.Normal.X = parseFloat(clearWords[0]);
                meshObject.Normal.Y = parseFloat(clearWords[1]);
                meshObject.Normal.Z = parseFloat(clearWords[2]);
                myObject.Meshes.push(meshObject);
            }
        }

        if (myObject.Meshes.length === 0) {
            throw 'Нет элементов';
        }

        return JSON.stringify(myObject);
    }
    catch (e){
        throw e;
    }
}


// Отрисовка
function drawLine(lineScene, material, beginPoint, endPoint, isEquidistant = false, isArea = false) {
    let points = [];
    points.push(beginPoint);
    points.push(endPoint);
    let lineGeometry = new THREE.BufferGeometry().setFromPoints(points);
    const line = new THREE.Line(lineGeometry, material);
    line.material.linewidth = 10;
    line.isequidistant = isEquidistant;
    line.isarea = isArea;
    lineScene.add(line);
}

function drawPlacedFigure() {
    let xhr = new XMLHttpRequest();
    xhr.open('GET', 'Home/GetPlacedScene', false);
    xhr.onload = function (e) {
        let figures = JSON.parse(xhr.response);
        let geometries = [];

        for (let figureId in figures) {
            let figure = figures[figureId];

            let meshColor = parseInt(figure.Color);

            for (let meshId in figure.Meshes) {
                let mesh = figure.Meshes[meshId];
                let geom = new THREE.BufferGeometry();

                let allCoords = [
                    mesh.FstPoint.X, mesh.FstPoint.Y, mesh.FstPoint.Z,
                    mesh.SndPoint.X, mesh.SndPoint.Y, mesh.SndPoint.Z,
                    mesh.TrdPoint.X, mesh.TrdPoint.Y, mesh.TrdPoint.Z
                ];

                let vertices = new Float32Array(allCoords);

                let normal = new Float32Array([
                    mesh.Normal.X, mesh.Normal.Y, mesh.Normal.Z,
                    mesh.Normal.X, mesh.Normal.Y, mesh.Normal.Z,
                    mesh.Normal.X, mesh.Normal.Y, mesh.Normal.Z
                ]);

                geom.addAttribute('position', new THREE.BufferAttribute(vertices, 3));
                geom.addAttribute('normal', new THREE.BufferAttribute(normal, 3));

                let color = new THREE.Color(meshColor);

                let rgb = color.toArray().map(v => v * 255);

                let numVerts = geom.getAttribute('position').count;
                let itemSize = 3;  // r, g, b
                let colors = new Uint8Array(itemSize * numVerts);

                colors.forEach((v, ndx) => {
                    colors[ndx] = rgb[ndx % 3];
                });

                let normalized = true;
                let colorAttrib = new THREE.BufferAttribute(colors, itemSize, normalized);
                geom.setAttribute('color', colorAttrib);

                geometries.push(geom);
            }
        }

        let mergedGeometry = mergeBufferGeometries(
            geometries, false);

        let material = new THREE.MeshStandardMaterial({
            vertexColors: true,
            wireframe: figureVisualParameters.frame
        });

        const mesh = new THREE.Mesh(mergedGeometry, material);
        scene.add(mesh);

        selectedMesh = mesh;

        render();
    };
    xhr.send();
}

function drawSelectedFigure() {
    let xhr = new XMLHttpRequest();
    xhr.open('GET', 'Home/GetSelectedFigure', false);
    xhr.onload = function (e) {
        let figure = JSON.parse(xhr.response);
        objectParameters.color = parseInt(figure.Color);

        let geometries = [];

        for (let meshId in figure.Meshes) {
            let mesh = figure.Meshes[meshId];
            let geom = new THREE.BufferGeometry();

            let allCoords = [
                mesh.FstPoint.X, mesh.FstPoint.Y, mesh.FstPoint.Z,
                mesh.SndPoint.X, mesh.SndPoint.Y, mesh.SndPoint.Z,
                mesh.TrdPoint.X, mesh.TrdPoint.Y, mesh.TrdPoint.Z
            ];

            let vertices = new Float32Array(allCoords);

            let normal = new Float32Array([
                mesh.Normal.X, mesh.Normal.Y, mesh.Normal.Z,
                mesh.Normal.X, mesh.Normal.Y, mesh.Normal.Z,
                mesh.Normal.X, mesh.Normal.Y, mesh.Normal.Z
            ]);

            geom.addAttribute('position', new THREE.BufferAttribute(vertices, 3));
            geom.addAttribute('normal', new THREE.BufferAttribute(normal, 3));

            let color = new THREE.Color(objectParameters.color);

            let rgb = color.toArray().map(v => v * 255);

            let numVerts = geom.getAttribute('position').count;
            let itemSize = 3;  // r, g, b
            let colors = new Uint8Array(itemSize * numVerts);

            colors.forEach((v, ndx) => {
                colors[ndx] = rgb[ndx % 3];
            });

            let normalized = true;
            let colorAttrib = new THREE.BufferAttribute(colors, itemSize, normalized);
            geom.setAttribute('color', colorAttrib);

            geometries.push(geom);
        }

        let mergedGeometry = mergeBufferGeometries(
            geometries, false);

        let material = new THREE.MeshStandardMaterial({
            vertexColors: true,
            wireframe: figureVisualParameters.frame
        });

        const mesh = new THREE.Mesh(mergedGeometry, material);
        mesh.figureId = figure.Id;
        scene.add(mesh);

        selectedMesh = mesh;

        objectParameters.angleX = figure.Rotation.X;
        objectParameters.angleY = figure.Rotation.Y;
        objectParameters.angleZ = figure.Rotation.Z;

        rotateSelectedMesh();

        render();
    }

    xhr.send();
}

function drawArea() {

    const material = new THREE.LineBasicMaterial({ color: 'white' });

    for (let i = 50; i < placingParameters.maxWidth; i += 50) {
        drawLine(scene, material, new THREE.Vector3(i, 0, 0), new THREE.Vector3(i, 0, placingParameters.maxHeight), false, true);
    }

    for (let j = 50; j < placingParameters.maxHeight; j += 50) {
        drawLine(scene, material, new THREE.Vector3(0, 0, j), new THREE.Vector3(placingParameters.maxWidth, 0, j), false, true);
    }

    drawLine(scene, material, new THREE.Vector3(0, 0, placingParameters.maxHeight), new THREE.Vector3(placingParameters.maxWidth, 0, placingParameters.maxHeight), false, true);
    drawLine(scene, material, new THREE.Vector3(placingParameters.maxWidth, 0, 0), new THREE.Vector3(placingParameters.maxWidth, 0, placingParameters.maxHeight), false, true);

    render();
}

function drawEquidistantFigure() {
    let xhr = new XMLHttpRequest();
    xhr.open('GET', 'Home/GetPlacedEcv', false);
    xhr.onload = function (e) {

        let figures = JSON.parse(xhr.response);

        for (let figureId in figures) {
            let ecv = figures[figureId];

            let ecvColor = ecv.Key;
            let points = ecv.Value;
            let xMaterial = new THREE.LineBasicMaterial({ color: parseInt(ecvColor) });

            for (let i = 0; i < points.length; i++) {
                let prevIndex;
                if (i == 0) {
                    prevIndex = points.length - 1;
                }
                else {
                    prevIndex = i - 1;
                }

                drawLine(scene, xMaterial, new THREE.Vector3(points[prevIndex].X, 0, points[prevIndex].Y), new THREE.Vector3(points[i].X, 0, points[i].Y), true);
            }
        }

        render();
    };
    xhr.send();
}

function drawAll() {
    let meshes = scene.children.filter((e) => {
        return e.type == 'Mesh' || (e.type == 'Line' && e.isequidistant);
    });

    for (let mesh in meshes) {
        scene.remove(meshes[mesh]);
    }

    if (isPlacing) {
        if (visualParameters.figure) {
            drawPlacedFigure();
        }
        if (visualParameters.equidistant) {
            drawEquidistantFigure();
        }
    } else {
        drawSelectedFigure();
    }

    render();
}


// Функции управлениия.
function setSelectedItem(id) {

    let xhr = new XMLHttpRequest();
    xhr.open('POST', 'Home/SetSelectedFigure', false);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onload = function() {
        drawAll();
    };
    xhr.send(id.toString());
}

function setMeshColor(mesh, color) {
    if (mesh != undefined) {
        let rgb = color.toArray().map(v => v * 255);

        let numVerts = mesh.geometry.attributes.color.length;
        let colors = new Uint8Array(numVerts);

        colors.forEach((v, ndx) => {
            colors[ndx] = rgb[ndx % 3];
        });

        let colorAttrib = new THREE.BufferAttribute(colors, 3, true);
        mesh.geometry.setAttribute('color', colorAttrib);

        render();
    }
}

function changeFigureVisualization() {
    if (selectedMesh != undefined) {
        selectedMesh.material.wireframe = figureVisualParameters.frame;

        render();
    }
}

function rotateSelectedMesh() {
    if (selectedMesh != undefined) {
        selectedMesh.rotation.x = 0.0;
        selectedMesh.rotation.y = 0.0;
        selectedMesh.rotation.z = 0.0;

        let xAxis = new THREE.Vector3(1.0, 0.0, 0.0);
        selectedMesh.rotateOnWorldAxis(xAxis, THREE.Math.degToRad(objectParameters.angleX));

        let yAxis = new THREE.Vector3(0.0, 1.0, 0.0);
        selectedMesh.rotateOnWorldAxis(yAxis, THREE.Math.degToRad(objectParameters.angleY));

        let zAxis = new THREE.Vector3(0.0, 0.0, 1.0);
        selectedMesh.rotateOnWorldAxis(zAxis, THREE.Math.degToRad(objectParameters.angleZ));

        render();
    }
}

function postRotate() {
    let content = objectParameters.angleX
        + " " + objectParameters.angleY
        + " " + objectParameters.angleZ;

    let xhr = new XMLHttpRequest();
    xhr.open('POST', 'Home/SetRotationSelectedFigure', false);
    xhr.send(content);
}

function colorSelectedMesh() {
    if (selectedMesh != undefined) {

        let color = new THREE.Color(objectParameters.color);
        setMeshColor(selectedMesh, color);

        render();
    }
}

function postColor() {
    let content = objectParameters.color;

    let xhr = new XMLHttpRequest();
    xhr.open('POST', 'Home/SetColorSelectedFigure', false);
    xhr.send(content);
}

function postPlaceParameters() {
    if (scene != undefined) {
        let meshes = scene.children.filter((e) => {
            return e.type == 'Line' && e.isarea;
        });

        for (let mesh in meshes) {
            scene.remove(meshes[mesh]);
        }

        drawArea();
    }

    let content = placingParameters.maxWidth
        + ' ' + placingParameters.maxHeight
        + ' ' + placingParameters.rotationAngle
        + ' ' + placingParameters.techDist;

    let xhr = new XMLHttpRequest();
    xhr.open('POST', 'Home/SetPlaceParameters', false);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(content);
}


// Работа с графикой.
function setCamera() {
    camera.position.x = radious * Math.sin(theta * Math.PI / 360) * Math.cos(phi * Math.PI / 360);
    camera.position.y = radious * Math.sin(phi * Math.PI / 360);
    camera.position.z = radious * Math.cos(theta * Math.PI / 360) * Math.cos(phi * Math.PI / 360);

    if (camera.position.x < 0 || camera.position.y < 0 || camera.position.z < 0) {
        return false;
    }

    return true;
}

function render() {
    camera.lookAt(0, 0, 0);
    renderer.render(scene, camera);
}

function progress(state) {
    let progress = document.getElementById("progress");

    if (state) {
        progress.classList.add('is-active');
    } else {
        progress.classList.remove('is-active');
    }
}

// Обработка событий
window.onload = function () {
    initGUI();

    placingFolder.hide();
    objectsFolder.hide();

    initGL();
}

window.onresize = function (){
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();

    renderer.setSize(window.innerWidth, window.innerHeight);
    render();
}

function onDocumentMouseWheel(event) {

    radious -= event.wheelDeltaY;

    let result = setCamera();

    if (!result) {
        radious += event.wheelDeltaY;
        setCamera();
    }

    camera.updateProjectionMatrix();
    render();
}

function onDocumentMouseMove(event) {

    event.preventDefault();

    if (isMouseDown) {

        theta = - ((event.clientX - onMouseDownPosition.x) * 0.5) + onMouseDownTheta;
        phi = ((event.clientY - onMouseDownPosition.y) * 0.5) + onMouseDownPhi;
        phi = Math.min(180, Math.max(0, phi));

        if (theta < 0) {
            theta = 0;
        }

        if (theta > 180) {
            theta = 180;
        }

        if (phi < 0) {
            phi = 0;
        }

        if (phi > 180) {
            phi = 180;
        }

        setCamera();

        camera.updateProjectionMatrix();
    }

    if (isMouseDown) {
        render();
    }
}

function onDocumentMouseDown(event) {
    event.preventDefault();

    if (event.target.tagName !== 'CANVAS') {
        return;
    }

    isMouseDown = true;

    onMouseDownTheta = theta;
    onMouseDownPhi = phi;
    onMouseDownPosition.x = event.clientX;
    onMouseDownPosition.y = event.clientY;
}

function onDocumentMouseUp(event) {
    event.preventDefault();

    if (!isMouseDown) {
        return;
    }

    isMouseDown = false;

    onMouseDownPosition.x = event.clientX - onMouseDownPosition.x;
    onMouseDownPosition.y = event.clientY - onMouseDownPosition.y;
}


// Работа с геометрией.
function mergeBufferGeometries(geometries, useGroups) {

    var isIndexed = geometries[0].index !== null;

    var attributesUsed = new Set(Object.keys(geometries[0].attributes));
    var morphAttributesUsed = new Set(Object.keys(geometries[0].morphAttributes));

    var attributes = {};
    var morphAttributes = {};

    var morphTargetsRelative = geometries[0].morphTargetsRelative;

    var mergedGeometry = new THREE.BufferGeometry();

    var offset = 0;

    for (var i = 0; i < geometries.length; ++i) {

        var geometry = geometries[i];
        var attributesCount = 0;

        // ensure that all geometries are indexed, or none

        if (isIndexed !== (geometry.index !== null)) {

            console.error('THREE.BufferGeometryUtils: .mergeBufferGeometries() failed with geometry at index ' + i + '. All geometries must have compatible attributes; make sure index attribute exists among all geometries, or in none of them.');
            return null;

        }

        // gather attributes, exit early if they're different

        for (var name in geometry.attributes) {

            if (!attributesUsed.has(name)) {

                console.error('THREE.BufferGeometryUtils: .mergeBufferGeometries() failed with geometry at index ' + i + '. All geometries must have compatible attributes; make sure "' + name + '" attribute exists among all geometries, or in none of them.');
                return null;

            }

            if (attributes[name] === undefined) attributes[name] = [];

            attributes[name].push(geometry.attributes[name]);

            attributesCount++;

        }

        // ensure geometries have the same number of attributes

        if (attributesCount !== attributesUsed.size) {

            console.error('THREE.BufferGeometryUtils: .mergeBufferGeometries() failed with geometry at index ' + i + '. Make sure all geometries have the same number of attributes.');
            return null;

        }

        // gather morph attributes, exit early if they're different

        if (morphTargetsRelative !== geometry.morphTargetsRelative) {

            console.error('THREE.BufferGeometryUtils: .mergeBufferGeometries() failed with geometry at index ' + i + '. .morphTargetsRelative must be consistent throughout all geometries.');
            return null;

        }

        for (var name in geometry.morphAttributes) {

            if (!morphAttributesUsed.has(name)) {

                console.error('THREE.BufferGeometryUtils: .mergeBufferGeometries() failed with geometry at index ' + i + '.  .morphAttributes must be consistent throughout all geometries.');
                return null;

            }

            if (morphAttributes[name] === undefined) morphAttributes[name] = [];

            morphAttributes[name].push(geometry.morphAttributes[name]);

        }

        // gather .userData

        mergedGeometry.userData.mergedUserData = mergedGeometry.userData.mergedUserData || [];
        mergedGeometry.userData.mergedUserData.push(geometry.userData);

        if (useGroups) {

            var count;

            if (isIndexed) {

                count = geometry.index.count;

            } else if (geometry.attributes.position !== undefined) {

                count = geometry.attributes.position.count;

            } else {

                console.error('THREE.BufferGeometryUtils: .mergeBufferGeometries() failed with geometry at index ' + i + '. The geometry must have either an index or a position attribute');
                return null;

            }

            mergedGeometry.addGroup(offset, count, i);

            offset += count;

        }

    }

    // merge indices

    if (isIndexed) {

        var indexOffset = 0;
        var mergedIndex = [];

        for (var i = 0; i < geometries.length; ++i) {

            var index = geometries[i].index;

            for (var j = 0; j < index.count; ++j) {

                mergedIndex.push(index.getX(j) + indexOffset);

            }

            indexOffset += geometries[i].attributes.position.count;

        }

        mergedGeometry.setIndex(mergedIndex);

    }

    // merge attributes

    for (var name in attributes) {

        var mergedAttribute = mergeBufferAttributes(attributes[name]);

        if (!mergedAttribute) {

            console.error('THREE.BufferGeometryUtils: .mergeBufferGeometries() failed while trying to merge the ' + name + ' attribute.');
            return null;

        }

        mergedGeometry.setAttribute(name, mergedAttribute);

    }

    // merge morph attributes

    for (var name in morphAttributes) {

        var numMorphTargets = morphAttributes[name][0].length;

        if (numMorphTargets === 0) break;

        mergedGeometry.morphAttributes = mergedGeometry.morphAttributes || {};
        mergedGeometry.morphAttributes[name] = [];

        for (var i = 0; i < numMorphTargets; ++i) {

            var morphAttributesToMerge = [];

            for (var j = 0; j < morphAttributes[name].length; ++j) {

                morphAttributesToMerge.push(morphAttributes[name][j][i]);

            }

            var mergedMorphAttribute = mergeBufferAttributes(morphAttributesToMerge);

            if (!mergedMorphAttribute) {

                console.error('THREE.BufferGeometryUtils: .mergeBufferGeometries() failed while trying to merge the ' + name + ' morphAttribute.');
                return null;

            }

            mergedGeometry.morphAttributes[name].push(mergedMorphAttribute);

        }

    }

    return mergedGeometry;
}

function mergeBufferAttributes(attributes) {

    var TypedArray;
    var itemSize;
    var normalized;
    var arrayLength = 0;

    for (var i = 0; i < attributes.length; ++i) {

        var attribute = attributes[i];

        if (attribute.isInterleavedBufferAttribute) {

            console.error('THREE.BufferGeometryUtils: .mergeBufferAttributes() failed. InterleavedBufferAttributes are not supported.');
            return null;

        }

        if (TypedArray === undefined) TypedArray = attribute.array.constructor;
        if (TypedArray !== attribute.array.constructor) {

            console.error('THREE.BufferGeometryUtils: .mergeBufferAttributes() failed. BufferAttribute.array must be of consistent array types across matching attributes.');
            return null;

        }

        if (itemSize === undefined) itemSize = attribute.itemSize;
        if (itemSize !== attribute.itemSize) {

            console.error('THREE.BufferGeometryUtils: .mergeBufferAttributes() failed. BufferAttribute.itemSize must be consistent across matching attributes.');
            return null;

        }

        if (normalized === undefined) normalized = attribute.normalized;
        if (normalized !== attribute.normalized) {

            console.error('THREE.BufferGeometryUtils: .mergeBufferAttributes() failed. BufferAttribute.normalized must be consistent across matching attributes.');
            return null;

        }

        arrayLength += attribute.array.length;

    }

    var array = new TypedArray(arrayLength);
    var offset = 0;

    for (var i = 0; i < attributes.length; ++i) {

        array.set(attributes[i].array, offset);

        offset += attributes[i].array.length;

    }

    return new THREE.BufferAttribute(array, itemSize, normalized);
}