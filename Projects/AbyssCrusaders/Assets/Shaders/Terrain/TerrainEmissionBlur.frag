#version 330

//This shader is optimized with expectation that emissionSpread will be more than occlusionSpread

uniform sampler2D inputMap;
uniform float occlusionSpread;
uniform float emissionSpread;
uniform float resolution;

in vec2 vUV;

out vec2 oResult;

void main (void)  
{
	vec2 pixelSize = vec2(1.0,1.0)/resolution;
    
    vec2 maxValues = texture2D(inputMap,vUV).xy;
	
	bool updateOcclusion = maxValues.x<1.0;
	bool updateEmission = maxValues.y<1.0;
    
    if(updateOcclusion || updateEmission) {
		float xyStart,xyEnd;
		if(updateEmission) {
			xyStart = -emissionSpread;
			xyEnd = emissionSpread;
		}else{
			xyStart = -occlusionSpread;
			xyEnd = occlusionSpread;
		}
		
    	for(float y = xyStart;y<=xyEnd;y++) {
			for(float x = xyStart;x<=xyEnd;x++) {
                if(x==0.0 && y==0.0) {
                    continue;
                }
                
    	        vec2 pixelPos = vec2(pixelSize.x*x,pixelSize.y*y);
				
				vec2 values = texture2D(inputMap,vUV+pixelPos).xy;
                
				//Emission
                if(updateOcclusion && values.x>=1.0 && (!updateEmission || (x>=-occlusionSpread && y>=-occlusionSpread && x<=occlusionSpread && y<=occlusionSpread))) {
                    float step = 1.0-(sqrt(x*x+y*y)/occlusionSpread);
                    
                    if(step>0.0) {
    	    			maxValues.x = max(maxValues.x,values.x*step*step);
                    }
                }
				
				//Emission
                if(updateEmission && values.y>=1.0) {
                    float step = 1.0-(sqrt(x*x+y*y)/emissionSpread);
                    
                    if(step>0.0) {
    	    			maxValues.y = max(maxValues.y,values.y*step*step);
                    }
                }
    	    }
    	}
    }
    
    oResult = maxValues;
	oResult.x = 1f-oResult.x;
}