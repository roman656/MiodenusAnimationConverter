using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.GeometryShaders
{
    public static class GridGeometryShader
    {
        public const ShaderType Type = ShaderType.GeometryShader;
        public const string Code = @"
                #version 330 core

                struct Grid
                {
                    vec3 position;
                    float cell_size;
                    int x_size_in_cells;
                    int y_size_in_cells;                   
                    int z_size_in_cells;
                    vec4 xy_plane_color;
                    vec4 xz_plane_color;
                    vec4 yz_plane_color;
                };
                
                layout (points) in;
                layout (line_strip, max_vertices = 256) out;

                out vec4 vertex_color;

                uniform mat4 view;
                uniform mat4 projection;
                uniform Grid grid;
                
                void create_grid(const in vec3 position, const in float cell_size, const in int x_size_in_cells,
                        const in int y_size_in_cells, const in int z_size_in_cells, const in vec4 xy_plane_color,
                        const in vec4 xz_plane_color, const in vec4 yz_plane_color);
                void create_coordinate_system(const in vec3 position, const in float x_axis_size,
                        const in float y_axis_size, const in float z_axis_size, const in vec4 x_axis_color,
                        const in vec4 y_axis_color, const in vec4 z_axis_color);               
              
                void main(void)
                {
                    create_grid(grid.position, grid.cell_size, grid.x_size_in_cells, grid.y_size_in_cells,
                            grid.z_size_in_cells, grid.xy_plane_color, grid.xz_plane_color, grid.yz_plane_color);
                    //create_coordinate_system(vec3(0.0f), 2.5f, 2.5f, 2.5f, vec4(1.0f, 0.0f, 0.0f, 1.0f), vec4(0.0f, 1.0f, 0.0f, 1.0f), vec4(0.0f, 0.0f, 1.0f, 1.0f));
                }

                void create_coordinate_system(const in vec3 position, const in float x_axis_size,
                        const in float y_axis_size, const in float z_axis_size, const in vec4 x_axis_color,
                        const in vec4 y_axis_color, const in vec4 z_axis_color)
                {
                    vertex_color = x_axis_color;
                    gl_Position = projection * view * vec4(position.x, position.y, position.z, 1.0f);
                    EmitVertex();
                    gl_Position = projection * view * vec4(position.x + x_axis_size, position.y, position.z, 1.0f);
                    EmitVertex();
                    EndPrimitive();

                    vertex_color = y_axis_color;
                    gl_Position = projection * view * vec4(position.x, position.y, position.z, 1.0f);
                    EmitVertex();
                    gl_Position = projection * view * vec4(position.x, position.y + y_axis_size, position.z, 1.0f);
                    EmitVertex();
                    EndPrimitive();

                    vertex_color = z_axis_color;
                    gl_Position = projection * view * vec4(position.x, position.y, position.z, 1.0f);
                    EmitVertex();
                    gl_Position = projection * view * vec4(position.x, position.y, position.z + z_axis_size, 1.0f);
                    EmitVertex();
                    EndPrimitive();
                }

                void create_grid(const in vec3 position, const in float cell_size, const in int x_size_in_cells,
                        const in int y_size_in_cells, const in int z_size_in_cells, const in vec4 xy_plane_color,
                        const in vec4 xz_plane_color, const in vec4 yz_plane_color)
                {    
                    float x_size = x_size_in_cells * cell_size;
                    float y_size = y_size_in_cells * cell_size;
                    float z_size = z_size_in_cells * cell_size;
                    
                    if (x_size_in_cells > 0 && z_size_in_cells > 0)
                    {
                        vertex_color = xz_plane_color;

                        for (int i = 0; i <= z_size_in_cells; i++)
                        {    
                            gl_Position = projection * view * vec4(position.x - x_size / 2.0f, position.y,
                                    position.z - z_size / 2.0f + cell_size * i, 1.0f);
                            EmitVertex();
                            gl_Position = projection * view * vec4(position.x + x_size / 2.0f, position.y,
                                    position.z - z_size / 2.0f + cell_size * i, 1.0f);
                            EmitVertex();
                            EndPrimitive();
                        }

                        for (int i = 0; i <= x_size_in_cells; i++)
                        {               
                            gl_Position = projection * view * vec4(position.x - x_size / 2.0f + cell_size * i,
                                    position.y, position.z - z_size / 2.0f, 1.0f);
                            EmitVertex();
                            gl_Position = projection * view * vec4(position.x - x_size / 2.0f + cell_size * i,
                                    position.y, position.z + z_size / 2.0f, 1.0f);
                            EmitVertex();
                            EndPrimitive();
                        }
                    }

                    if (x_size_in_cells > 0 && y_size_in_cells > 0)
                    {
                        vertex_color = xy_plane_color;

                        for (int i = 0; i <= y_size_in_cells; i++)
                        {
                            gl_Position = projection * view * vec4(position.x - x_size / 2.0f,
                                    position.y - y_size / 2.0f + cell_size * i, position.z, 1.0f);
                            EmitVertex();
                            gl_Position = projection * view * vec4(position.x + x_size / 2.0f,
                                    position.y - y_size / 2.0f + cell_size * i, position.z, 1.0f);
                            EmitVertex();
                            EndPrimitive();
                        }

                        for (int i = 0; i <= x_size_in_cells; i++)
                        {
                            gl_Position = projection * view * vec4(position.x - x_size / 2.0f + cell_size * i,
                                    position.y - y_size / 2.0f, position.z, 1.0f);
                            EmitVertex();
                            gl_Position = projection * view * vec4(position.x - x_size / 2.0f + cell_size * i,
                                    position.y + y_size / 2.0f, position.z, 1.0f);
                            EmitVertex();
                            EndPrimitive();
                        }
                    }

                    if (y_size_in_cells > 0 && z_size_in_cells > 0)
                    {
                        vertex_color = yz_plane_color;

                        for (int i = 0; i <= y_size_in_cells; i++)
                        {
                            gl_Position = projection * view * vec4(position.x,
                                    position.y - y_size / 2.0f + cell_size * i, position.z - z_size / 2.0f, 1.0f);
                            EmitVertex();
                            gl_Position = projection * view * vec4(position.x,
                                    position.y - y_size / 2.0f + cell_size * i, position.z + z_size / 2.0f, 1.0f);
                            EmitVertex();
                            EndPrimitive();
                        }

                        for (int i = 0; i <= z_size_in_cells; i++)
                        {
                            gl_Position = projection * view * vec4(position.x, position.y - y_size / 2.0f,
                                    position.z - z_size / 2.0f + cell_size * i, 1.0f);
                            EmitVertex();
                            gl_Position = projection * view * vec4(position.x, position.y + y_size / 2.0f,
                                    position.z - z_size / 2.0f + cell_size * i, 1.0f);
                            EmitVertex();
                            EndPrimitive();
                        }
                    }
                }
                ";
    }
}