uniform sampler2D texture;
uniform float r;
uniform float g;
uniform float b;

void main()
{
    vec2 coord = gl_TexCoord[0].xy;

    // lookup the pixel in the texture
    // vec4 pixel = texture2D(texture, gl_TexCoord[0].xy);

    gl_FragColor = vec4(r, g, b, 1);
}