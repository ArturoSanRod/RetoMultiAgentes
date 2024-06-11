from mesa import Agent, Model
from mesa.space import MultiGrid
from mesa.time import SimultaneousActivation
from http.server import BaseHTTPRequestHandler, HTTPServer
import logging
import json

# Definición de agentes trash, robot
class TrashAgent(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.trash_amount = 0

class RobotAgent(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.storage = 0
        self.capacity = 5

    def step(self):
        # Ejemplo simple de comportamiento
        self.move()
        self.collect_trash()

    def move(self):
        possible_steps = self.model.grid.get_neighborhood(self.pos, moore=True, include_center=False)
        new_position = self.random.choice(possible_steps)
        self.model.grid.move_agent(self, new_position)

    def collect_trash(self):
        cell_contents = self.model.grid.get_cell_list_contents([self.pos])
        for agent in cell_contents:
            if isinstance(agent, TrashAgent) and agent.trash_amount > 0:
                while self.storage < self.capacity and agent.trash_amount > 0:
                    self.storage += 1
                    agent.trash_amount -= 1
                if self.storage == self.capacity:
                    self.move_to_trash_bin()

    def move_to_trash_bin(self):
        trash_bin_pos = self.model.trash_bin_pos
        self.model.grid.move_agent(self, trash_bin_pos)
        self.storage = 0

# Modelo de simulación 
class TrashModel(Model):
    def __init__(self, height, width, trash_map):
        self.grid = MultiGrid(width, height, True)
        self.schedule = SimultaneousActivation(self)
        self.trash_bin_pos = None
        
        # Crear celdas de basura y obstáculos
        for (content, (x, y)) in trash_map.items():
            if content == 'X':
                continue  # Ignorar celdas bloqueadas
            trash_amount = int(content) if content.isdigit() else 0
            if content == 'P':
                self.trash_bin_pos = (x, y)
            agent = TrashAgent((x, y), self)
            self.grid.place_agent(agent, (x, y))
            agent.trash_amount = trash_amount

        # Agregar robots
        self.robots = []
        for i in range(5):
            robot = RobotAgent(i, self)
            self.schedule.add(robot)
            self.grid.place_agent(robot, (0, 0))  # Ubicación inicial de los robots
            self.robots.append(robot)

    def step(self):
        self.schedule.step()

# Función para leer el archivo de entrada
def read_input_file(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()
        n, m = map(int, lines[0].strip().split())
        trash_map = {}
        for i in range(1, n + 1):
            row = lines[i].strip().split()
            for j in range(len(row)):
                trash_map[row[j]] = (j, i - 1)
        return n, m, trash_map

# Servidor HTTP para comunicación con Unity
class Server(BaseHTTPRequestHandler):
    
    def _set_response(self):
        self.send_response(200)
        self.send_header('Content-type', 'application/json')
        self.end_headers()
        
    def do_GET(self):
        self._set_response()
        self.wfile.write(json.dumps({"status": "server running"}).encode('utf-8'))

    def do_POST(self):
        content_length = int(self.headers['Content-Length'])
        post_data = self.rfile.read(content_length)
        data = json.loads(post_data)

        # Ejecutar un paso de la simulación
        model.step()

        # Obtener las posiciones de todos los robots en cada paso
        positions = [{"id": robot.unique_id, "x": robot.pos[0], "y": robot.pos[1]} for robot in model.robots]

        self._set_response()
        self.wfile.write(json.dumps(positions).encode('utf-8'))

def run(server_class=HTTPServer, handler_class=Server, port=8585):
    logging.basicConfig(level=logging.INFO)
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    logging.info("Starting httpd...\n")
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        pass
    httpd.server_close()
    logging.info("Stopping httpd...\n")

# Ejemplo de uso
if __name__ == '__main__':
    file_path = "inicial.txt"
    height, width, trash_map = read_input_file(file_path)
    model = TrashModel(height, width, trash_map)
    
    from sys import argv
    if len(argv) == 2:
        run(port=int(argv[1]))
    else:
        run()
