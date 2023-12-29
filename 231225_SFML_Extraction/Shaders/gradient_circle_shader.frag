// gradient_circle_shader.frag

// 정밀도를 조절할 수 있습니다.
#define PRECISION 100.0

void main()
{
    // 화면 좌표
    vec2 position = gl_FragCoord.xy;

    // 원의 중앙 좌표
    vec2 center = vec2(0.0, 0.0);

    // 원의 반지름
    float radius = 100.0;

    // 원의 중앙에서 현재 픽셀까지의 거리 계산
    float distance = length(position - center);

    // 거리가 반지름보다 작으면 내부에 위치한 픽셀이므로 흰색
    if (distance < radius)
    {
        gl_FragColor = vec4(1.0, 1.0, 1.0, 1.0);
    }
    else
    {
        // 원의 테두리에서부터 외부까지의 거리 비율 계산
        float ratio = (distance - radius) / PRECISION;

        // 그라데이션 효과 생성 (투명 -> 흰색)
        gl_FragColor = vec4(1.0, 1.0, 1.0, 1.0 - ratio);
    }
}