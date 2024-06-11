import random
from mesa import Agent, Model
from mesa.time import RandomActivation
from mesa.space import MultiGrid
import json

# Para el servidor
from http.server import BaseHTTPRequestHandler, HTTPServer
import logging
from sys import argv


class espacioModel(Model):
    def __init__(self, width, height, num_agents, inicial):
        self.grid = MultiGrid(width, height, True)
        self.schedule = RandomActivation(self)
        self.num_agents = num_agents
        self.pasos = 0
        self.running = True
        self.positions_per_step = []

        # Crear agentes y obstáculos estado inicial
        self.cantidad_botes = 0
        for y in range(height):
            for x in range(width):
                if inicial[y][x] == "X":
                    obstaculo = Obstaculo((x, y), self)
                    self.grid.place_agent(obstaculo, (x, y))
                elif inicial[y][x] == "P":
                    self.bote_basura = BoteBasura((x, y), self)
                    self.grid.place_agent(self.bote_basura, (x, y))
                else:
                    cantidad_botes = int(inicial[y][x])
                    if cantidad_botes > 0:
                        self.cantidad_botes += cantidad_botes
                        basura = Basura((x, y), cantidad_botes, self)
                        self.grid.place_agent(basura, (x, y))

        for i in range(self.num_agents):
            x, y = self.find_empty()
            agente = espacioAgent(i, self, (x, y))
            self.schedule.add(agente)
            self.grid.place_agent(agente, (x, y))

    def step(self):
        self.schedule.step()
        self.pasos += 1
        self.positions_per_step.append(self.get_positions())
        if self.cantidad_botes == 0:
            self.running = False

    def find_empty(self):
        empty_cells = [
            (x, y)
            for x in range(self.grid.width)
            for y in range(self.grid.height)
            if self.grid.is_cell_empty((x, y))
        ]
        return random.choice(empty_cells)

    # Regresa posiciones de todos los agentes en el grid
    def get_positions(self):
        matriz = [["0" for _ in range(self.grid.width)] for _ in range(self.grid.height)]

        for (cell_content, (x, y)) in self.grid.coord_iter():
            if any(isinstance(obj, espacioAgent) for obj in cell_content):
                matriz[y][x] = "A"
            elif any(isinstance(obj, Obstaculo) for obj in cell_content):
                matriz[y][x] = "X"
            elif any(isinstance(obj, BoteBasura) for obj in cell_content):
                matriz[y][x] = "P"
            elif any(isinstance(obj, Basura) for obj in cell_content):
                basura = next(obj for obj in cell_content if isinstance(obj, Basura))
                matriz[y][x] = str(basura.cantidad)
        return matriz


class Obstaculo(Agent):
    def __init__(self, pos, model):
        super().__init__(pos, model)
        self.pos = pos


class BoteBasura(Agent):
    def __init__(self, pos, model):
        super().__init__(pos, model)
        self.pos = pos


class Basura(Agent):
    def __init__(self, pos, cantidad, model):
        super().__init__(pos, model)
        self.pos = pos
        self.cantidad = cantidad


class espacioAgent(Agent):
    def __init__(self, unique_id, model, pos):
        super().__init__(unique_id, model)
        self.unique_id = unique_id
        self.pos = pos
        self.capacidad = 5
        self.recolectada = 0

    def avanza(self):
        posibles_movimientos = self.model.grid.get_neighborhood(
            self.pos, moore=True, include_center=False
        )
        movimientos_validos = [step for step in posibles_movimientos if self.model.grid.is_cell_empty(step) or isinstance(self.model.grid.get_cell_list_contents([step])[0], Basura)]

        if len(movimientos_validos) > 0:
            new_position = random.choice(movimientos_validos)
            self.model.grid.move_agent(self, new_position)

    def recoge_basura(self):
        cellmates = self.model.grid.get_cell_list_contents([self.pos])
        for obj in cellmates:
            if isinstance(obj, Basura) and self.recolectada < self.capacidad:
                self.recolectada += obj.cantidad
                self.model.cantidad_botes -= obj.cantidad
                self.model.grid.remove_agent(obj)
                if self.recolectada >= self.capacidad:
                    break

    def deposita_basura(self):
        self.model.grid.move_agent(self, self.model.bote_basura.pos)
        self.recolectada = 0

    def step(self):
        self.avanza()
        self.recoge_basura()
        if self.recolectada == self.capacidad:
            self.deposita_basura()


def run_model(filename):
    with open(filename) as f:
        data = f.readlines()

    height, width = map(int, data[0].split())
    inicial = [line.split() for line in data[1:]]

    model = espacioModel(width, height, 5, inicial)

    steps_data = []

    # Nuevo formato json para unity web client
    while model.running:
        model.step()
        steps_data.append({
            "Dimensions": [height, width],
            "Terrain": model.get_positions()
        })

    return steps_data


data = run_model('inicial.txt')
datajson = json.dumps(data[-1])
# Imprimir en la terminal arreglo de cada paso
print(datajson)

class Server(BaseHTTPRequestHandler):
    def _set_response(self):
        self.send_response(200)
        self.send_header('Content-type', 'application/json')
        self.end_headers()

    def do_GET(self):
        self._set_response()
        self.wfile.write('GET request for {}'.format(self.path).encode('utf-8'))

    def do_POST(self):
        self._set_response()
        self.wfile.write(datajson.encode('utf-8'))


def run(server_class=HTTPServer, handler_class=Server, port=8585):
    logging.basicConfig(level=logging.INFO)
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    logging.info("Starting server...\n")
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        pass
    httpd.server_close()
    logging.info("Stopping server...\n")

if __name__ == "__main__":
    if len(argv) == 2:
        run(port=int(argv[1]))
    else:
        run()
