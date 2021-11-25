float time;
float glow;
int sphere=1;
float yrot;
float rand (float2 p)
{
    return frac(sin(dot(p.xy,float2(12389.1283,8941.1283)))*(12893.128933));
}

float hash( float n )
{
    return frac(sin(n)*43758.5453);
}

float smin(float d1, float d2, float k)
{
    float h = clamp(0.5+0.5*(d2-d1)/k,0.0,1.0);
    return lerp(d2,d1,h)-k*h*(1.0-h);
}
float box(float3 p, float3 b)
{
    float3 q = abs(p) - b;
    return min(max(q.x,max(q.y,q.z)),0.0)+length(max(q,0.0));
}

float2x2 rot(float x)
{
    x %= (3.1415927 * 2);
    float s = sin(x);
    float c = cos(x);
    return float2x2(c, -s, s, c);
}

float map(float3 p)
{
    p.xz=mul(p.xz,rot(yrot));
    p.yz=mul(p.yz,rot(3.1415927/8.0));
    float r = 01;
    float3 pp = p;
    for(int i = 0; i < 6; i++)
    {
        p.xy = mul(p.xy, rot(i+time*1.5));
        p.yz = mul(p.yz, rot(1.284+time*2.+0.238));
        p.xz = mul(p.xz, rot(.238+i+time*0.2));
                    
        p=abs(p);

        p.yz+=0.01;
        p.xz-=0.075;
        p.x+=0.025;
        float b = box(p, float3(.001,.001,.1));
        r=smin(r, b,0.005);
        glow += 0.05/(0.05+b*b);
    }

    if(sphere==1)
        r=smin(r,length(pp)-0.001,0.1);
                
    return r;
}

            
float2 raymarch(float3 p, float3 dir)
{
    float t = 0.0;
    int STEPS = 32;
    float MIN_DIST = 0.001;
    glow = 0;
    for(int i=0; i<STEPS; i++)
    {
        float dist = map(p);

        if(dist < MIN_DIST)
        {
            return float2(1,t);
        }
        t += dist;

        p += dir * dist;
    }
                
    return float2(0, t);
}

float3 normal(float3 p)
{
    float2 e = float2(0.0005, 0.);
    return normalize(float3(
        map(p-e.xyy)-map(p+e.xyy),
        map(p-e.yxy)-map(p+e.yxy),
        map(p-e.yyx)-map(p+e.yyx)));
}
